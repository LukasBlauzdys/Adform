# Squares API

## Task

Create an API that from a given set of points in a 2D plane - enables the consumer to find out sets of points that make squares and how many squares can be drawn. A point is a pair of integer X and Y coordinates. A square is a set of 4 points that when connected make up, well, a square.


## Base URL

- Local: `http://localhost:8080/swagger/`

## Setup
### Running application
`docker-compose up --build`

### Services
 - API: http://localhost:8080
 - Swagger: http://localhost:8080/Swagger
 - MongoDB: mongodb://localhost:27017
 - Prometheus: http://localhost:9090
 - Grafana: http://localhost:3000
### Default logins
 - MongoDB: root/psw1
 - Grafana: admin/yourpassword
## Endpoints

### Points

#### Get All Points

- **GET** `/api/Points`
- Description: Retrieves all points from the database.
- Response: List of points.

#### Create Points

- **POST** `/api/Points`
- Description: Creates new points in the database.
- Request Body: List of points to create. `[{"x": 0,"y": 0}]`
- Response: Confirmation message.

#### Delete Point by Coordinates

- **DELETE** `/api/Points`
- Description: Deletes a specific point based on its coordinates.
- Request Body: Point object with coordinates to delete. `{"x": 0,"y": 0}`
- Response: No content.

#### Get Point by ID

- **GET** `/api/Points/{id}`
- Description: Retrieves a specific point by its ID.
- Parameters:
  - `id` (string): The ID of the point.
- Response: The requested point.

#### Update Point by ID

- **PUT** `/api/Points/{id}`
- Description: Updates an existing point in the database.
- Parameters:
  - `id` (string): The ID of the point to update.
- Request Body: The updated point object. ``{"x": 0,"y": 0}``
- Response: No content.

#### Delete Point by ID

- **DELETE** `/api/Points/{id}`
- Description: Deletes a specific point by its ID.
- Parameters:
  - `id` (string): The ID of the point to delete.
- Response: No content.

### Squares

#### Count Squares with Details

- **GET** `/api/Points/Squares`
- Description: Retrieves the count and detailed information of all squares formed by the points.
- Response: Count and details of squares.

#### Count Squares

- **GET** `/api/Points/CountSquares`
- Description: Retrieves the count of all squares formed by the points.
- Response: Count of squares.

## Notes
MongoDB and user is initialized with init-mongo.js

