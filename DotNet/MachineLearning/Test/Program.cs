using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using numl.Model;
using numl.Supervised.DecisionTree;
using numl.Supervised.KNN;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var description = Descriptor.Create<Iris>();

            var generator = new KNNGenerator();
            var data = Iris.Load();
            var model = generator.Generate(description, data);
            Console.WriteLine(model);
            

            var iris = new Iris()
            {
                SepalLength = 4.6m,
                SepalWidth = 3.1m,
                PetalLength = 1.5m,
                PetalWidth = 0.2m
            };
            var pIris = model.Predict(iris);
            Console.ReadLine();
        }
    }
}
