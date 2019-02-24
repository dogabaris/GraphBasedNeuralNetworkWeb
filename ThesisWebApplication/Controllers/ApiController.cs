using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using ThesisWebApplication.Helpers;
using ThesisWebApplication.Models;
using ThesisWebApplication.Perceptron;

namespace ThesisWebApplication.Controllers
{
    internal class Perceptron
    {
        public int Id { get; set; }
        public dynamic Data { get; set; }
        public Guid TryId { get; set; }
    }

    internal class Connection
    {
        public double[] Weight { get; set; }
        public Guid TryId { get; set; }
    }
    public class YsaController : ApiController
    {
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/test")]
        // GET api/<controller>
        public string Test()
        {
            return "Başarılı!";
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/train")]
        public IHttpActionResult Train()
        {
            var client = Neo4JHelper.ConnectDb();
            //Db sıfırlama.
            //client.Cypher
            //    .OptionalMatch("(n)-[r]-()")
            //    .With("n,r")
            //    .Delete("n,r")
            //    .ExecuteWithoutResults();

            // training set for an NAND function
            //TrainingItem[] trainingSet =
            //{
            //    new TrainingItem(true, 1, 0, 0),
            //    //new TrainingItem(true, 1, 0, 1),
            //    //new TrainingItem(true, 1, 1, 0),
            //    //new TrainingItem(false, 1, 1, 1)
            //};

            TrainingItem[] inputPerceptron = {
                new TrainingItem(true, 1, 0, 0),
                //new TrainingItem(true, 1, 0, 1),
                //new TrainingItem(true, 1, 1, 0),
               // new TrainingItem(false, 1, 1, 1)
            };
            client.Cypher
                .Create("(n:Input {data})")
                .WithParam("data", new Perceptron { Id = 0, TryId = Globals.TryId, Data = inputPerceptron[0].Inputs[0] })
                .ExecuteWithoutResults();
            client.Cypher
                .Create("(n:Input {data})")
                .WithParam("data", new Perceptron { Id = 1, TryId = Globals.TryId, Data = inputPerceptron[0].Inputs[1] })
                .ExecuteWithoutResults();
            client.Cypher
                .Create("(n:Input {data})")
                .WithParam("data", new Perceptron { Id = 2, TryId = Globals.TryId, Data = inputPerceptron[0].Inputs[2] })
                .ExecuteWithoutResults();

            client.Cypher
                .Create("(n:Hidden {data})")
                .WithParam("data", new Perceptron { Id = 3, TryId = Globals.TryId, Data = "NAND" })
                .ExecuteWithoutResults();

            client.Cypher
                .Create("(n:Output {data})")
                .WithParam("data", new Perceptron { Id = 4, TryId = Globals.TryId, Data = inputPerceptron[0].Output })
                .ExecuteWithoutResults();

            // create a perceptron with 3 inputs
            var perceptron = new BinaryPerceptron(3);
            var returnJson = new JObject();

            int attemptCount = 0;
            // teach the neural network until all the inputs are correctly clasified
            while (true)
            {
                Console.WriteLine("-- Attempt: " + (++attemptCount));

                int errorCount = 0;
                foreach (var item in inputPerceptron)
                {
                    // teach the perceptron to which class given inputs belong
                    var output = perceptron.Learn(item.Output, item.Inputs);
                    // check that the inputs were classified correctly
                    if (output != item.Output)
                    {
                        returnJson.Add(String.Format("Fail{0}", attemptCount), String.Join(",", item.Inputs[0], item.Inputs[1], item.Inputs[2], output));
                        Console.WriteLine("Fail\t {0} & {1} & {2} != {3}", item.Inputs[0], item.Inputs[1], item.Inputs[2], output);
                        errorCount++;
                    }
                    else
                    {
                        returnJson.Add(String.Format("Pass{0}", attemptCount), String.Join(",", item.Inputs[0], item.Inputs[1], item.Inputs[2], output));
                        Console.WriteLine("Pass\t {0} & {1} & {2} = {3}", item.Inputs[0], item.Inputs[1], item.Inputs[2], output);
                    }
                }

                // only quit when there were no unexpected outputs detected
                if (errorCount == 0)
                {
                    return Ok(returnJson);
                    //break;
                }
            }
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}