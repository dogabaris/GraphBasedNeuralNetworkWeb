using System;
using System.Linq;
using Neo4jClient;
using ThesisWebApplication.Controllers;
using ThesisWebApplication.Helpers;

namespace ThesisWebApplication.Perceptron
{
    public class BinaryPerceptron
    {
        public double LearningRate { set; get; }

        public double[] Weights { set; get; }

        public double Threshold { set; get; }

        public GraphClient Client { get; set; }

        public BinaryPerceptron(int inputCount, double learningRate = 0.1, double threshold = 0.5)
        {
            Weights = new double[inputCount];
            LearningRate = learningRate;
            Threshold = threshold;

            Client = Neo4JHelper.ConnectDb();
            var connection = new Connection
            {
                TryId = Globals.TryId,
                Weight = new[] { 0.0 }
            };

            Client.Cypher
                .Match("(perceptron1:Input)", "(perceptron2:Hidden)")
                    .Where((Controllers.Perceptron perceptron1) => perceptron1.Id == 0 && perceptron1.TryId == Globals.TryId)
                    .AndWhere((Controllers.Perceptron perceptron2) => perceptron2.Id == 3 && perceptron2.TryId == Globals.TryId)
                    .Create("(perceptron1)-[:CONNECTED {connection}]->(perceptron2)")
                    .WithParam("connection", connection)
                    .ExecuteWithoutResults();

            Client.Cypher
                .Match("(perceptron1:Input)", "(perceptron2:Hidden)")
                    .Where((Controllers.Perceptron perceptron1) => perceptron1.Id == 1 && perceptron1.TryId == Globals.TryId)
                    .AndWhere((Controllers.Perceptron perceptron2) => perceptron2.Id == 3 && perceptron2.TryId == Globals.TryId)
                    .Create("(perceptron1)-[:CONNECTED {connection}]->(perceptron2)")
                    .WithParam("connection", connection)
                    .ExecuteWithoutResults();

            Client.Cypher
                .Match("(perceptron1:Input)", "(perceptron2:Hidden)")
                    .Where((Controllers.Perceptron perceptron1) => perceptron1.Id == 2 && perceptron1.TryId == Globals.TryId)
                    .AndWhere((Controllers.Perceptron perceptron2) => perceptron2.Id == 3 && perceptron2.TryId == Globals.TryId)
                    .Create("(perceptron1)-[:CONNECTED {connection}]->(perceptron2)")
                    .WithParam("connection", connection)
                    .ExecuteWithoutResults();

            Client.Cypher
                .Match("(perceptron1:Hidden)", "(perceptron2:Output)")
                    .Where((Controllers.Perceptron perceptron1) => perceptron1.Id == 3 && perceptron1.TryId == Globals.TryId)
                    .AndWhere((Controllers.Perceptron perceptron2) => perceptron2.Id == 4 && perceptron2.TryId == Globals.TryId)
                    .Create("(perceptron1)-[:CONNECTED]->(perceptron2)")
                    .ExecuteWithoutResults();
        }

        public bool GetResult(params double[] inputs)
        {
            if (inputs.Length != Weights.Length)
                throw new ArgumentException("Invalid number of inputs. Expected: " + Weights.Length);

            // perceptronun outputunu hesaplar ve threshold ile bool değer döner
            return DotProduct(inputs, Weights) > Threshold;
        }

        public bool Learn(bool expectedResult, params double[] inputs)
        {
            // get the result
            bool result = GetResult(inputs);

            // if the result does not match expected
            if (result != expectedResult)
            {
                // calculate error (need to convert boolean to a number)
                double error = (expectedResult ? 1 : 0) - (result ? 1 : 0);
                for (int i = 0; i < Weights.Length; i++)
                {
                    // adjust the weights
                    Weights[i] += LearningRate * error * inputs[i];
                    // Neo4j weight updates
                    var iCopy = i;
                    //if (i == 0)
                    //{
                    //    Client.Cypher
                    //        .Match("(inp:Input)-[res:CONNECTED]->(hid:Hidden)")
                    //        .Where((Perceptron inp) => inp.Id == iCopy && inp.TryId == Globals.TryId)
                    //        .AndWhere((Perceptron hid) => hid.Id == 3 && hid.TryId == Globals.TryId)
                    //        .Set("res.Weight = res.Weight + {UpdatedWeight}")
                    //        .WithParam("UpdatedWeight", LearningRate * error * inputs[iCopy])
                    //        .ExecuteWithoutResults();
                    //}
                    //else
                    //{//{Iterator}
                    Client.Cypher
                    .Match("(inp:Input)-[res:CONNECTED]->(hid:Hidden)")
                    .Where((Controllers.Perceptron inp) => inp.Id == iCopy && inp.TryId == Globals.TryId)
                    .AndWhere((Controllers.Perceptron hid) => hid.Id == 3 && hid.TryId == Globals.TryId)
                    .Set("res.Weight = res.Weight + {UpdatedWeight}")
                    .WithParams(new
                    {
                        UpdatedWeight = LearningRate * error * inputs[iCopy]
                    })
                    //.WithParam("UpdatedWeight", LearningRate * error * inputs[i])
                    //.WithParam("iterator", i-1)
                    .ExecuteWithoutResults();
                    //}

                }
            }

            return result;
        }

        private double DotProduct(double[] inputs, double[] weights)
        {
            return inputs.Zip(weights, (value, weight) => value * weight).Sum();
        }
    }
}
