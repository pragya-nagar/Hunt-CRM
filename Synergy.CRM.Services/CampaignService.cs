using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Synergy.Common.Exceptions;
using Synergy.Common.FileStorage.Abstraction;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.DAL.Commands.Queries;
using Synergy.CRM.Models.Commands;
using Synergy.ServiceBus.Abstracts;
using Synergy.ServiceBus.Extensions.Progress;

namespace Synergy.CRM.Services
{
    public class CampaignService :
        IMessageHandler<CampaignCreateCommand>,
        IMessageHandler<CampaignUpdateCommand>,
        IMessageHandler<CampaignCommentCreateCommand>,
        IMessageHandler<CampaignCommentUpdateCommand>,
        IMessageHandler<CampaignCommentDeleteCommand>,
        IMessageHandler<MakeCampaignDataDumpCommand>,
        IMessageHandler<CreateRuleCommand>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IPublishMessage _serviceBus;
        private readonly IProgressPublisher _progressPublisher;
        private readonly IFileStorage _fileStorage;
        private readonly ICreateCampaignCommand _createCampaignCommand;
        private readonly IUpdateCampaignCommand _updateCampaignCommand;
        private readonly ICreateCampaignCommentCommand _createCampaignCommentCommand;
        private readonly IUpdateCampaignCommentCommand _updateCampaignCommentCommand;
        private readonly IDeleteCampaignCommentCommand _deleteCampaignCommentCommand;

        private readonly UserExistsQuery _userExistsQuery;
        private readonly CampaignExistsQuery _campaignExistsQuery;
        private readonly CampaignQuery _campaignQuery;
        private readonly CampaignLeadDumpQuery _campaignLeadDumpQuery;
        private readonly CampaignPropertyDumpQuery _campaignPropertyDumpQuery;
        private readonly GetCampaignCommentAuthorIdQuery _getCampaignCommentAuthorIdQuery;
        private readonly GetCampaignRulesAndCountiesQuery _getCampaignRulesAndCountiesQuery;
        private readonly ICreateCampaignGeneralRuleCommand _createCampaignRuleCommand;

        public CampaignService(ILogger<CampaignService> logger,
            IMapper mapper,
            IPublishMessage serviceBus,
            IProgressPublisher progressPublisher,
            IFileStorage fileStorage,
            ICreateCampaignCommand createCampaignCommand,
            IUpdateCampaignCommand updateCampaignCommand,
            ICreateCampaignCommentCommand createCampaignCommentCommand,
            IUpdateCampaignCommentCommand updateCampaignCommentCommand,
            IDeleteCampaignCommentCommand deleteCampaignCommentCommand,
            ICreateCampaignGeneralRuleCommand createCampaignRuleCommand,
            UserExistsQuery userExistsQuery,
            CampaignQuery campaignQuery,
            CampaignExistsQuery campaignExistsQuery,
            CampaignLeadDumpQuery campaignLeadDumpQuery,
            CampaignPropertyDumpQuery campaignPropertyDumpQuery,
            GetCampaignCommentAuthorIdQuery getCampaignCommentAuthorIdQuery,
            GetCampaignRulesAndCountiesQuery getCampaignRulesAndCountiesQuery)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            this._progressPublisher = progressPublisher ?? throw new ArgumentNullException(nameof(progressPublisher));
            this._fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            this._createCampaignCommand = createCampaignCommand ?? throw new ArgumentNullException(nameof(createCampaignCommand));
            this._updateCampaignCommand = updateCampaignCommand ?? throw new ArgumentNullException(nameof(updateCampaignCommand));
            this._createCampaignCommentCommand = createCampaignCommentCommand ?? throw new ArgumentNullException(nameof(createCampaignCommentCommand));
            this._updateCampaignCommentCommand = updateCampaignCommentCommand ?? throw new ArgumentNullException(nameof(updateCampaignCommentCommand));
            this._deleteCampaignCommentCommand = deleteCampaignCommentCommand ?? throw new ArgumentNullException(nameof(deleteCampaignCommentCommand));
            this._createCampaignRuleCommand = createCampaignRuleCommand ?? throw new ArgumentNullException(nameof(createCampaignRuleCommand));

            this._userExistsQuery = userExistsQuery ?? throw new ArgumentNullException(nameof(userExistsQuery));
            this._campaignQuery = campaignQuery ?? throw new ArgumentNullException(nameof(campaignQuery));
            this._campaignExistsQuery = campaignExistsQuery ?? throw new ArgumentNullException(nameof(campaignExistsQuery));
            this._campaignLeadDumpQuery = campaignLeadDumpQuery ?? throw new ArgumentNullException(nameof(campaignLeadDumpQuery));
            this._campaignPropertyDumpQuery = campaignPropertyDumpQuery ?? throw new ArgumentNullException(nameof(campaignPropertyDumpQuery));
            this._getCampaignCommentAuthorIdQuery = getCampaignCommentAuthorIdQuery ?? throw new ArgumentNullException(nameof(getCampaignCommentAuthorIdQuery));
            this._getCampaignRulesAndCountiesQuery = getCampaignRulesAndCountiesQuery ?? throw new ArgumentNullException(nameof(getCampaignRulesAndCountiesQuery));
        }

        public void Handle(CampaignCreateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(CampaignCreateCommand message, CancellationToken cancellationToken = default)
        {
            var cmd = this._mapper.Map<CreateCampaignModel>(message);
            await this._createCampaignCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(CampaignUpdateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(CampaignUpdateCommand message, CancellationToken cancellationToken = default)
        {
            var exists = await this._campaignExistsQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new NotFoundException($"Campaign with id '{message.Id}' does not exist");
            }

            exists = await this._userExistsQuery.ExecuteAsync(message.AssignedToUserId, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new ModelStateException(nameof(message.AssignedToUserId), $"User with id '{message.AssignedToUserId}' does not exist");
            }

            var campaignRulesAndCounties = await this._getCampaignRulesAndCountiesQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);

            var cmd = this._mapper.Map<UpdateCampaignModel>(message);
            await this._updateCampaignCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);

            if (message.CountyIds.Except(campaignRulesAndCounties.CountyIds).Any()
                || campaignRulesAndCounties.CountyIds.Except(message.CountyIds).Any()
                || message.StateId != campaignRulesAndCounties.StateId)
            {
                var assignmentCommand = Command.Create<ApplyRulesCommand>(Guid.NewGuid(), message.CreatedBy);
                assignmentCommand.CampaignId = message.Id;
                assignmentCommand.RuleIds = campaignRulesAndCounties.RuleIds;

                await this._serviceBus.PublishAsync(assignmentCommand, cancellationToken).ConfigureAwait(false);
            }
        }

        public void Handle(CampaignCommentCreateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(CampaignCommentCreateCommand message, CancellationToken cancellationToken = default)
        {
            var exists = await this._campaignExistsQuery.ExecuteAsync(message.CampaignId, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new NotFoundException($"Campaign with id '{message.CampaignId}' does not exist");
            }

            var cmd = this._mapper.Map<CreateCampaignCommentModel>(message);
            await this._createCampaignCommentCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(CampaignCommentUpdateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(CampaignCommentUpdateCommand message, CancellationToken cancellationToken = default)
        {
            var author = await this._getCampaignCommentAuthorIdQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);

            if (author == null)
            {
                throw new NotFoundException();
            }

            if (author != message.CreatedBy)
            {
                throw new NotAcceptableException("Only author can alter comment.");
            }

            await this._updateCampaignCommentCommand.DispatchAsync(new UpdateCampaignCommentModel
                    {
                        Id = message.Id,
                        Comment = message.Comment,
                    }, message.CreatedBy,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public void Handle(CampaignCommentDeleteCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(CampaignCommentDeleteCommand message, CancellationToken cancellationToken = default)
        {
            var author = await this._getCampaignCommentAuthorIdQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);

            if (author == null)
            {
                throw new NotFoundException();
            }

            if (author != message.CreatedBy)
            {
                throw new NotAcceptableException("Only author can delete comment.");
            }

            var command = new DeleteCampaignCommentModel { Id = message.Id };
            await this._deleteCampaignCommentCommand
                .DispatchAsync(command, message.CreatedBy, cancellationToken)
                .ConfigureAwait(false);
        }

        public void Handle(MakeCampaignDataDumpCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(MakeCampaignDataDumpCommand message, CancellationToken cancellationToken = default)
        {
            var keyParams = message.Key.Split(':');
            if (keyParams.Length != 5 || keyParams[0] != "campaigns" || keyParams[2] != "dumps")
            {
                throw new ModelStateException(nameof(message.Key), "Incorrect key");
            }

            var campaign = await this._campaignQuery.ExecuteAsync(message.CampaignId, cancellationToken).ConfigureAwait(false);
            if (campaign == null)
            {
                throw new NotFoundException($"Campaign with id '{message.CampaignId}' does not exist");
            }

            await this._progressPublisher.PostProgressAsync(5, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var columns = message.Fields.OrderBy(x => x.Order).ToList();

            IList<IDictionary<string, object>> rows;
            switch (keyParams[3])
            {
                case "lead":
                    {
                        var list = await this._campaignLeadDumpQuery.ExecuteAsync(message.CampaignId, cancellationToken).ConfigureAwait(false);
                        rows = list.ToList();
                        break;
                    }

                case "property":
                    {
                        var list = await this._campaignPropertyDumpQuery.ExecuteAsync(message.CampaignId, cancellationToken).ConfigureAwait(false);
                        rows = list.ToList();
                        break;
                    }

                default:
                    throw new ModelStateException(nameof(message.Key), "Incorrect key");
            }

            if (rows.Count == 0)
            {
                throw new NotFoundException("There are no records to process");
            }

            await this._progressPublisher.PostProgressAsync(20, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Dump");
                for (var i = 0; i < columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = string.IsNullOrWhiteSpace(columns[i].Alias) == true ? columns[i].Key : columns[i].Alias;
                }

                for (var i = 0; i < rows.Count; i++)
                {
                    for (var j = 0; j < columns.Count; j++)
                    {
                        var key = columns[j].Key;
                        if (rows[i].ContainsKey(key) == false)
                        {
                            continue;
                        }

                        var val = rows[i][key];

                        var cell = worksheet.Cells[i + 2, j + 1];
                        cell.Value = val;

                        switch (val)
                        {
                            case string _:
                                cell.Style.Numberformat.Format = "@";
                                break;
                            case DateTime _:
                                cell.Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo?.ShortDatePattern;
                                break;
                            default:
                                {
                                    if (long.TryParse(Convert.ToString(val, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _) == true)
                                    {
                                        cell.Style.Numberformat.Format = "#";
                                    }
                                    else if (decimal.TryParse(Convert.ToString(val, CultureInfo.InvariantCulture), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out _) == true)
                                    {
                                        cell.Style.Numberformat.Format = "0.00";
                                    }

                                    break;
                                }
                        }
                    }
                }

                await this._progressPublisher.PostProgressAsync(70, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                var data = package.GetAsByteArray();
                await this._fileStorage.SaveAsync(data, $"{message.Key.Replace(":", "/")}.xlsx", cancellationToken).ConfigureAwait(false);
            }
        }

        public void Handle(CreateRuleCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(CreateRuleCommand message, CancellationToken cancellationToken = default)
        {
            var args = this._mapper.Map<CreateRuleModel>(message);
            await this._createCampaignRuleCommand.DispatchAsync(args, message.CreatedBy, cancellationToken).ConfigureAwait(false);
            this._logger.LogInformation("New rule with name - {Name} has been added", args.Name);
        }
    }
}
