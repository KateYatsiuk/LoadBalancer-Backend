# Web app for calculating trigonometric functions using Taylor's series

## Educational project for familiarization with load balancer and other approaches to building web applications

## Features:

- Client authorization
- Since requests are time-consuming, the maximum labor intensity of one task is limited (number of additions). The input data of the task is validated both on the frontend and backend sides.
- The client has access to the history of task execution results, can view the status of the current task, cancel task execution, start execution of a new task.
- The app provides load balancing for different amount of servers.

## Architecture of backend
The backend is implemented using 3 layer architecture:
- **Presentation layer** - client part, fromntend (React)
- **Business Logic Layer (BLL)** - the part, where the general logic of the project is implemented
- **Data Access Logic (DAL)** - the layer, which connects the logic with the database
<img src="docs\imgs\3-layer-architecture.jpg" alt="Watch the series" width="200" height="180" border="10" />

## Load balancing logic
1. Steps here
