using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;

namespace YouTubeBot.YouTube
{
    public class YouTubeEngine
    {
        //TO GET THE API KEY, USE GOOGLE CLOUD CONSOLE
        private readonly string channelId = "INSERT-CHANNELID-HERE";
        private readonly string apiKey = "INSERT-API-KEY-HERE";

        public YouTubeVideo GetLatestVideo()
        {
            //Temporary variables for Video info
            string videoId;
            string videoUrl;
            string videoTitle;
            DateTime? videoPublishedAt;

            //Initializing the API
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "MyApp"
            });

            //Setting up our video search query
            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.ChannelId = channelId;
            searchListRequest.MaxResults = 1;
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            //Executing the search
            var searchListResponse = searchListRequest.Execute();

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video") //We are looking for a youtube video here
                {
                    videoId = searchResult.Id.VideoId; //Setting our details
                    videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                    videoTitle = searchResult.Snippet.Title;
                    videoPublishedAt = searchResult.Snippet.PublishedAt;
                    var thumbnail = searchResult.Snippet.Thumbnails.Default__.Url;

                    return new YouTubeVideo() //Storing in a class for use in the bot
                    {
                        videoId = videoId,
                        videoUrl = videoUrl,
                        videoTitle = videoTitle,
                        thumbnail = thumbnail,
                        PublishedAt = videoPublishedAt
                    };
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}
