﻿using ElasticTest.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ElasticTest.Controllers
{
    public class elasticDemoController : ApiController
    {
        // GET: api/elasticDemo
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/elasticDemo/5
        public List<tweet> Get(string title)
        {

            // Connect tới database ở cloud
            var connectionSettings = new ConnectionSettings(new Uri("https://14cd3e6bc2a32141a8adde7f202b630e.us-east-1.aws.found.io:9243"));
            // Đây là username/password của cloud đó.
            connectionSettings.BasicAuthentication("elastic", "SzdD4h7CfuNiL9HvMmyJ4l7z");
            // Connect
            var elasticClient = new ElasticClient(connectionSettings);

            // Query search cho index có dạng:
            // account / user / 1
            // {
            //      "user" : "tri",
            //      "post_date" : "2017-03/11",
            //      "message" : "Hi, first user"
            // }

            // old search
            //var searchResponse = elasticClient.Search<tweet>(sd => sd
            //                                                .Index("post")
            //                                                .Type("home")
            //                                                .Query(q => q
            //                                                    .MultiMatch(m => m.Fields(f => f.Field(p => p.title).Field(p => p.description).Field(p => p.district)).Query(title)
            //                                                    )));
            // new search
            var searchResponse = elasticClient.Search<tweet>(sd => sd
                                                            .Index("post")
                                                            .Type("home")
                                                            .Query(q => q.Wildcard(c => c
                                                            .Field(p => p.description)
                                                            .Value("*" + title + "*")
                                                            )));
            // trả về
            List <tweet> obj = new List<tweet>();
            if (searchResponse.IsValid && searchResponse.Hits.Count != 0)
            {
                obj = searchResponse.Documents.ToList();
            }
            else
            {
                obj[0].title = "search title: " + title;
                obj[0].description = "empty";
                obj[0].district = "district not found!";
                obj[0].image = "image not found!";
            }
            return obj;
        }

        // POST: api/elasticDemo
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/elasticDemo/5
        public void Put(int id, [FromBody]string value)
        {
            // Connect tới database ở cloud
            var connectionSettings = new ConnectionSettings(new Uri("https://14cd3e6bc2a32141a8adde7f202b630e.us-east-1.aws.found.io:9243"));
            // Đây là username/password của cloud đó.
            connectionSettings.BasicAuthentication("elastic", "SzdD4h7CfuNiL9HvMmyJ4l7z");
            // Connect
            var elasticClient = new ElasticClient(connectionSettings);

            // Query search cho index có dạng:
            // account / user / 1
            // {
            //      "user" : "tri",
            //      "post_date" : "2017-03/11",
            //      "message" : "Hi, first user"
            // }
            var searchResponse = elasticClient.Search<tweet>(sd => sd
                                                            .Index("account") // index dòng 51
                                                            .Type("user") // type dòng 52
                                                            .Size(10000) // số lượng lấy
                                                            .Query(q => q
                                                                .Match(m => m.Field("user").Query("tri") // kết quả sẽ match với field "user" có giá trị "tri"
                                                                )));

            //return searchResponse;
        }

        // DELETE: api/elasticDemo/5
        public void Delete(int id)
        {
        }
    }
}
