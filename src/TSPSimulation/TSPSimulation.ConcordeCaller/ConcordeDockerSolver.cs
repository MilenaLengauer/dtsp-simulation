using Docker.DotNet;
using Docker.DotNet.Models;
using Serilog;
using TSPSimulation.ProblemDefinition.Static;

namespace TSPSimulation.ExternalSolvers
{
    public class ConcordeDockerSolver : ISolver
    {

        public string Id { get; }
        public TSP Problem { get; }

        public ConcordeDockerSolver(string id, TSP problem)
        {
            Id = id;
            Problem = problem;
        }

        public int[] Solve()
        {
            var task = SolveAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<int[]> SolveAsync()
        {
            Log.Logger.Information("Calling concorde solver...");
            var dir = Directory.CreateDirectory("tmp\\concorde");

            // write tsp file
            var dateFormat = "yyyyMMddHHmmssffff";
            var fileName = $"{Id}{DateTime.Now.ToString(dateFormat)}";
            var tspFilePath = Path.Combine(dir.FullName, $"{fileName}.tsp");
            TSPFileWriter.WriteFile(tspFilePath, Id, Problem);

            var client = new DockerClientConfiguration().CreateClient();

            // create container
            var response = await client
                .Containers
                .CreateContainerAsync(new CreateContainerParameters()
                {
                    Image = "concorde",
                    Env = new[] { $"INPUT_FILE=/root/tsps/{fileName}.tsp", $"OUTPUT_FILE=/root/tsps/{fileName}.sol" },
                    HostConfig = new HostConfig()
                    {
                        Binds = new[] { $"{dir.FullName}:/root/tsps/" }
                    }
                });

            // run container
            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());

            await client.Containers.WaitContainerAsync(response.ID);

            await client.Containers.RemoveContainerAsync(response.ID, new ContainerRemoveParameters());

            // read file tmp\\concorde\\{Id}.sol
            var concordeResultPath = Path.Combine(dir.FullName, $"{fileName}.sol");
            int[] result = ConcordeResultFileReader.ReadResult(concordeResultPath, Problem);
            File.Delete(tspFilePath);
            File.Delete(concordeResultPath);
            return result;
        }

    }
}
