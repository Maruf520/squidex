﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschränkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Threading.Tasks;
using CoreTweet;
using Microsoft.Extensions.Options;
using Squidex.Domain.Apps.Core.HandleRules.EnrichedEvents;
using Squidex.Domain.Apps.Core.Rules.Actions;
using Squidex.Infrastructure;

#pragma warning disable SA1649 // File name must match first type name

namespace Squidex.Domain.Apps.Core.HandleRules.Actions
{
    public sealed class TweetJob
    {
        public string AccessToken { get; set; }

        public string AccessSecret { get; set; }

        public string Text { get; set; }
    }

    public sealed class TweetActionHandler : RuleActionHandler<TweetAction, TweetJob>
    {
        private const string Description = "Send a tweet";

        private readonly RuleEventFormatter formatter;
        private readonly TwitterOptions twitterOptions;

        public TweetActionHandler(RuleEventFormatter formatter, IOptions<TwitterOptions> twitterOptions)
        {
            Guard.NotNull(formatter, nameof(formatter));
            Guard.NotNull(twitterOptions, nameof(twitterOptions));

            this.formatter = formatter;

            this.twitterOptions = twitterOptions.Value;
        }

        protected override (string Description, TweetJob Data) CreateJob(EnrichedEvent @event, TweetAction action)
        {
            var text = formatter.Format(action.Text, @event);

            var ruleJob = new TweetJob
            {
                Text = text,
                AccessToken = action.AccessToken,
                AccessSecret = action.AccessSecret
            };

            return (Description, ruleJob);
        }

        protected override async Task<(string Dump, Exception Exception)> ExecuteJobAsync(TweetJob job)
        {
            try
            {
                var tokens = Tokens.Create(
                    twitterOptions.ClientId,
                    twitterOptions.ClientSecret,
                    job.AccessToken,
                    job.AccessSecret);

                 var response = await tokens.Statuses.UpdateAsync(status => job.Text);

                return ($"Tweeted: {job.Text}", null);
            }
            catch (Exception ex)
            {
                return (ex.Message, ex);
            }
        }
    }
}
