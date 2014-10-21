using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml;

namespace dbnet2.Models.Widgets.BuildMonitor
{
    public class TeamCityBuildMonitor
    {
        HttpClient Client { get; set; }
        public TeamCityBuildMonitor(string baseUri)
        {
            Client = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };

            var user = ConfigurationManager.AppSettings["teamCityUser"];
            var password = ConfigurationManager.AppSettings["teamCityPass"];

            //Client.DefaultRequestHeaders.Add("Accept","application/json");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes(string.Format("{0}:{1}", user, password))
                )
            );

        }
        public BuildMonitorSummary GetSummary()
        {
            var summary = new BuildMonitorSummary {Projects = GetProjects()};

            foreach (var project in summary.Projects)
            {
                var buildStatus = GetBuildStatus(project);
                if (buildStatus != null && buildStatus.Any())
                {
                    project.Builds.AddRange(buildStatus);
                }
            }

            return summary;
        }

        public List<Project> GetProjects()
        {
            var xml = Client.GetStringAsync("/httpAuth/app/rest/projects").Result;

            var projectSummary = new XmlDocument();
            projectSummary.LoadXml(xml);

            var projects = (from XmlNode projectItem in projectSummary.SelectNodes("//project")
                let xmlAttributeCollection = projectItem.Attributes
                where xmlAttributeCollection != null
                select new Project
                {
                    Id = xmlAttributeCollection["id"].Value, 
                    Name = xmlAttributeCollection["name"].Value, 
                    Description = xmlAttributeCollection["description"] != null ? xmlAttributeCollection["description"].Value : "No Description",
                    Href = xmlAttributeCollection["webUrl"].Value
                }).ToList();

            return projects;
        }

        public List<BuildStatus> GetBuildStatus(Project project)
        {
            // get project information
            var projectSummaryXml = Client.GetStringAsync(String.Format("/httpAuth/app/rest/projects/id:{0}", project.Id)).Result;
            var projectSummaryDoc = new XmlDocument();
            projectSummaryDoc.LoadXml(projectSummaryXml);

            // get all builds
            var builds = new List<BuildStatus>();
            var buildNodes = projectSummaryDoc.SelectNodes("//buildType");

            // add to collection
            foreach(XmlNode node in buildNodes)
            {
                var buildStatus = new BuildStatus
                {
                    BuildName = node.Attributes["name"].Value, 
                    Id = node.Attributes["id"].Value
                };

                GetBuildDetails(buildStatus);
                builds.Add(buildStatus);
            }

            return builds;
        }

        public BuildStatus GetBuildDetails(BuildStatus build)
        {
            var xml = Client.GetStringAsync(String.Format("/httpAuth/app/rest/buildTypes/id:{0}/builds", build.Id)).Result;
            
            var buildSummary = new XmlDocument();
            buildSummary.LoadXml(xml);

            // get build details
            var latest = buildSummary.SelectSingleNode("//build");

            build.BuildNumber = int.Parse(latest.Attributes["id"].Value);
            build.State = (latest.Attributes["status"].Value.ToLowerInvariant() == "success"
                ? BuildStatus.BuildState.Successful
                : BuildStatus.BuildState.Failed);

            // get latest build XML
            var latestBuildXml = Client.GetStringAsync(String.Format("/httpAuth/app/rest/buildTypes/id:{0}/builds/id:{1}", build.Id, build.BuildNumber)).Result;
            var latestBuildDoc = new XmlDocument();
            latestBuildDoc.LoadXml(latestBuildXml);

            var latestBuildDate = latestBuildDoc.SelectSingleNode("//finishDate");
            build.LastBuild = DateTime.ParseExact(latestBuildDate.InnerText, "yyyyMMdd'T'HHmmsszzz", CultureInfo.InvariantCulture);

            return build;
        }
    }

 

    public class BuildMonitorSummary
    {
        public List<Project> Projects { get; set; }
    }

    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Href { get; set; }
        public List<BuildStatus> Builds = new List<BuildStatus>();
    }

    public class BuildStatus
    {
        public enum BuildState
        {
            Unknown,
            Failed,
            Successful
        }
        public string Id { get; set; }
        public string BuildName { get; set; }
        public int BuildNumber { get; set; }
        public BuildState State { get; set; }
        public DateTime LastBuild { get; set; }
        public DateTime LastSuccessfulBuild { get; set; }

    }
}
