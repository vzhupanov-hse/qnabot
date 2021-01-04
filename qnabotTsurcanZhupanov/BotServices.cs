// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class BotServices : IBotServices
    {

        /// <summary>
        /// Constructor create new instance of the QnAMaker class
        /// </summary>
        /// <param name="configuration"></param>
        public BotServices(IConfiguration configuration)
        {
            QnAMakerService = new QnAMaker(new QnAMakerEndpoint
            {
                KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                EndpointKey = configuration["QnAAuthKey"],
                Host = GetHostname(configuration["QnAEndpointHostName"])
            });
        }

        public QnAMaker QnAMakerService { get; private set; }

        /// <summary>
        /// Method return name of the right host
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns>hostname</returns>
        private static string GetHostname(string hostname)
        {
            if (!hostname.StartsWith("https://"))
            {
                hostname = string.Concat("https://", hostname);
            }

            if (!hostname.EndsWith("/qnamaker"))
            {
                hostname = string.Concat(hostname, "/qnamaker");
            }

            return hostname;
        }
    }
}
