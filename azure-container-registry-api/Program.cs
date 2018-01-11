using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace azure_container_registry_api
{
    class Program
    {
        static readonly string AzureContainerRegistryUsername = "{CHANGE ME}";
        static readonly string AzureContainerRegistryPassword = "{CHANGE ME}";
        static readonly string AzureContainerRegistryUrl = "https://{CHANGE ME}.azurecr.io";

        static void Main(string[] args)
        {
            List<TagsListResponse> tagsList = new List<TagsListResponse>();

            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(
                                                           Encoding.ASCII.GetBytes(String.Format("{0}:{1}",
                                                               AzureContainerRegistryUsername,
                                                               AzureContainerRegistryPassword))
                                                       ));

                //Get repositories
                string repositoriesResult = webClient.DownloadString(AzureContainerRegistryUrl + "/v2/_catalog");
                if (!string.IsNullOrEmpty(repositoriesResult))
                {
                    RepositoriesResponse repositoriResponseItem = JsonConvert.DeserializeObject<RepositoriesResponse>(repositoriesResult);

                    //Get tags for every repository.
                    foreach (string repository in repositoriResponseItem.Repositories)
                    {
                        string tagsListResult = webClient.DownloadString(AzureContainerRegistryUrl + $"/v2/{repository}/tags/list");

                        if (string.IsNullOrEmpty(tagsListResult)) continue;

                        tagsList.Add(JsonConvert.DeserializeObject<TagsListResponse>(tagsListResult));
                    }
                }
            }

            Console.WriteLine($"Found {tagsList.Count} repositories.");
            Console.ReadLine();
        }
        class RepositoriesResponse
        {
            [JsonProperty(PropertyName = "repositories")]
            public string[] Repositories { get; set; }
        }
        class TagsListResponse
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
            [JsonProperty(PropertyName = "tags")]
            public string[] Tags { get; set; }
        }
    }
}
