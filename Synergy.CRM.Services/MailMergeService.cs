using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Synergy.Common.Exceptions;
using Synergy.Common.FileStorage.Abstraction;
using Synergy.CRM.DAL.Commands.Models.Results.MailMerge;
using Synergy.CRM.DAL.Commands.Queries;
using Synergy.CRM.Models.Commands;
using Synergy.DataAccess.Enum;
using Synergy.ServiceBus.Abstracts;
using Synergy.ServiceBus.Extensions.Progress;
using Synergy.ServiceBus.Messages;

namespace Synergy.CRM.Services
{
    public class MailMergeService : IMessageHandler<MailMergeCommand>, IMessageHandler<MailMergeFinishedEvent>
    {
        private readonly IMapper _mapper;
        private readonly IPublishMessage _publisher;
        private readonly IFileStorage _fileStorage;

        private readonly GetMailMergePropertyFieldsQuery _mailMergePropertyFieldsQuery;
        private readonly GetMailMergeTemplateQuery _mailMergeTemplateQuery;

        public MailMergeService(IMapper mapper,
                                IPublishMessage publisher,
                                IFileStorage fileStorage,
                                GetMailMergePropertyFieldsQuery mailMergePropertyFieldsQuery,
                                GetMailMergeTemplateQuery mailMergeTemplateQuery)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this._fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            this._mailMergePropertyFieldsQuery = mailMergePropertyFieldsQuery ?? throw new ArgumentNullException(nameof(mailMergePropertyFieldsQuery));
            this._mailMergeTemplateQuery = mailMergeTemplateQuery ?? throw new ArgumentNullException(nameof(mailMergeTemplateQuery));
        }

        public void Handle(MailMergeCommand message)
        {
            this.HandleAsync(message, new CancellationToken(false)).Wait();
        }

        public async Task HandleAsync(MailMergeCommand message, CancellationToken cancellationToken)
        {
            var propertyIdList = await this.GetPropertyIdList(message.PropertyPath, cancellationToken).ConfigureAwait(false);

            if (propertyIdList.Any() == false)
            {
                throw new NotAcceptableException("Provided Internal Property ID’s not found. Please double-check.");
            }

            var template = await this._mailMergeTemplateQuery.ExecuteAsync(message.TemplateId, cancellationToken).ConfigureAwait(false);
            if (template == null)
            {
                throw new NotAcceptableException("Merge template not found");
            }

            if (string.IsNullOrEmpty(template.FileId))
            {
                throw new NotAcceptableException("Merge template file id not found");
            }

            var propertyFields = await this._mailMergePropertyFieldsQuery.ExecuteAsync((propertyIdList, message.CampaignId), cancellationToken).ConfigureAwait(false);
            if (propertyFields.Count() != propertyIdList.Count())
            {
                throw new NotAcceptableException("Please double-check Internal Property Id’s. Some of them are not relevant.");
            }

            var mergeSingleFieldsList = this._mapper.Map<IEnumerable<MergeSingleFields>>(propertyFields);

            IEnumerable<MergeFields> mergeFieldsList;
            if (template.GroupingType == (int)MergeFieldsGroupingType.PerOwner)
            {
                var groups = mergeSingleFieldsList.GroupBy(x => x.Owner);
                mergeFieldsList = groups.Select(g => this._mapper.Map<MergeFields>(g.ToList())).ToList();
            }
            else
            {
                var groups = mergeSingleFieldsList.GroupBy(x => x.InternalDelinquencyId);
                mergeFieldsList = groups.Select(g => this._mapper.Map<MergeFields>(g.ToList())).ToList();
            }

            var evt = Event.Create<MailMergeStartedEvent>(message.Id, message.CreatedBy);
            evt.ResultPath = message.ResultPath;
            evt.TemplateFilePath = template.FileId.Replace(':', '/');
            evt.MergeFields = mergeFieldsList;
            evt.Source = "CRM";
            await this._publisher.PublishAsync(evt, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(MailMergeFinishedEvent message)
        {
            this.HandleAsync(message, default).GetAwaiter().GetResult();
        }

        public async Task HandleAsync(MailMergeFinishedEvent message, CancellationToken cancellationToken)
        {
            if (message.Source == "CRM")
            {
                var evt = ServiceBus.Abstracts.Event.Create<OperationStatusEvent>(Guid.NewGuid(), message.CreatedBy);
                evt.Code = HttpStatusCode.OK;
                evt.Message = "OK";

                await this._publisher.PublishAsync(evt, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task<List<Guid>> GetPropertyIdList(string delinquencyPath, CancellationToken cancellationToken)
        {
            var delinquencyIds = new List<Guid>();
            var fileContent = await this._fileStorage.GetAsync(delinquencyPath, cancellationToken).ConfigureAwait(false);
            using (var memoryStream = new MemoryStream(fileContent))
            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new NotAcceptableException("Provided Internal Property ID’s not found. Please double-check.");
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;

                int startRow = 1;
                var firstPlainId = worksheet.Cells[startRow, 1].Value;
                if (firstPlainId != null && Guid.TryParse(firstPlainId.ToString(), out Guid firstParsedId) == true)
                {
                    delinquencyIds.Add(firstParsedId);
                }

                startRow = 2;
                for (var rowIndex = startRow; rowIndex <= rowCount; ++rowIndex)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var plainId = worksheet.Cells[rowIndex, 1].Value;
                    if (plainId == null)
                    {
                        continue;
                    }

                    if (Guid.TryParse(plainId.ToString(), out var parsedId) == false)
                    {
                        throw new NotAcceptableException($"Internal Property ID {plainId} has unknown format");
                    }

                    delinquencyIds.Add(parsedId);
                }
            }

            return delinquencyIds.Distinct().ToList();
        }
    }
}
