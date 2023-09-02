# DTSP Simulation

The goal of this program is to simulate dynamic travelling salesperson environments and solve them online with different genetic algorithms. The provided algorithms include the following:

- Standard genetic algorithm
- Elitism-based immigrants genetic algorithm
- Random immigrants genetic algorithm
- Age-layered population structure genetic algorithm
- Neighbourhood genetic algorithm


## Setup

In order to run the application it is required to setup a docker image with the name `concorde` that provides the Concorde solver.

For that, run the following command in `./src/docker`:

```
docker build -t concorde .
```

To execute the simulation run the WPF application (`TSPSimulation.UserInterface`). The application runs on .NET 6.0.

## Running the application

The user interface for this simulation is provided as a WPF application. The main steps users have to take to simulate and evaluate a DTSP environment are:

- Configuring the problem: define the problem configuration in JSON format and load it in the application (see `./experiments/setup/problems` for examples)
- Configuring the algorithm: define one or multiple algorithms in a JSON file (see `./experiments/setup/algorithms` for examples)
- Start the simulation
- Evaluate the runs and compare them to each other

For the configuration of problems, it is necessary to specify a static base problem from the TSPLIB. However, the program can only handle files in the format `EUC_2D`. The files have to be provided within the executing directory of the application under the folder `./TSPs`. The following are already provided in this directory: berlin10, berlin52, ch150, eil51, eil76, eil101, kroA200, pcb442.