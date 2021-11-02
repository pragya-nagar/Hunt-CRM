using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.DAL.Commands.Queries;
using Synergy.CRM.Models.Commands.Reminder;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Services
{
    public class ReminderService : IMessageHandler<ReminderCreateCommand>, IMessageHandler<ReminderUpdateCommand>, IMessageHandler<ReminderUpdateNotificationCommand>
    {
        private readonly IMapper _mapper;
        private readonly ICreateReminderCommand _createReminderCommand;
        private readonly IUpdateReminderNotificationCommand _updateReminderNotificationCommand;
        private readonly ReminderExistQuery _reminderExistQuery;
        private readonly GetReminderQuery _getReminderQuery;
        private readonly IUpdateReminderCommand _updateReminderCommand;

        public ReminderService(IMapper mapper,
            ICreateReminderCommand createReminderCommand,
            IUpdateReminderNotificationCommand updateReminderNotificationCommand,
            ReminderExistQuery reminderExistQuery,
            GetReminderQuery getReminderQuery,
            IUpdateReminderCommand updateReminderCommand)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._createReminderCommand =
                createReminderCommand ?? throw new ArgumentNullException(nameof(createReminderCommand));
            this._updateReminderNotificationCommand = updateReminderNotificationCommand ?? throw new ArgumentNullException(nameof(updateReminderNotificationCommand));
            this._reminderExistQuery = reminderExistQuery ?? throw new ArgumentNullException(nameof(reminderExistQuery));
            this._getReminderQuery = getReminderQuery ?? throw new ArgumentNullException(nameof(getReminderQuery));
            this._updateReminderCommand = updateReminderCommand ?? throw new ArgumentNullException(nameof(updateReminderCommand));
            this._reminderExistQuery = reminderExistQuery ?? throw new ArgumentNullException(nameof(reminderExistQuery));
        }

        public void Handle(ReminderCreateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(ReminderCreateCommand message, CancellationToken cancellationToken = default)
        {
            var cmd = this._mapper.Map<CreateReminderModel>(message);

            await this._createReminderCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken)
                .ConfigureAwait(false);
        }

        public void Handle(ReminderUpdateNotificationCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(ReminderUpdateNotificationCommand message, CancellationToken cancellationToken = default)
        {
            var exists = await this._reminderExistQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new NotFoundException($"Reminder with id '{message.Id}' does not exist");
            }

            var reminder = await this._getReminderQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);
            if (message.IsEmailNotification == null)
            {
                message.IsEmailNotification = reminder.IsEmailNotification;
            }

            if (message.IsPushNotification == null)
            {
                message.IsPushNotification = reminder.IsPushNotification;
            }

            if (message.Status == null)
            {
                message.Status = reminder.Status;
            }

            var cmd = this._mapper.Map<UpdateReminderNotificationModel>(message);

            await this._updateReminderNotificationCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(ReminderUpdateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(ReminderUpdateCommand message, CancellationToken cancellationToken = default)
        {
            var exists = await this._reminderExistQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new NotFoundException($"Reminder with id '{message.Id}' does not exist");
            }

            var reminder = await this._getReminderQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);

            var sheduledDate = reminder.SheduledDate;
            var sheduledTime = reminder.SheduledTime;
            var status = reminder.Status;

            var newDateTime = sheduledDate.Add(sheduledTime);

            if (newDateTime < DateTime.Now && status == 0)
            {
                var cmd = this._mapper.Map<UpdateReminderModel>(message);

                await this._createReminderCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var cmd = this._mapper.Map<UpdateReminderModel>(message);

                await this._updateReminderCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
