# SwitchBoardAPI

### Introduction
SwitchBoardApi efficiently manages computational simulation resources. The API will handle the spawning and monitoring of Docker containers for incoming simulation jobs, allowing each job to be executed in its own container. In addition, the API allows the simulations' results to be saved to a persistent storage volume, ensuring that the results are accessible even after the container has been removed.

### SwitchBoardAPI Features
1. Create or spawn a container
2. Get status of all the containers
3. Stop/Delete a container

### Installation Guide
* Clone this repository
* The main branch is the most stable branch at any given time, ensure you're working from it.

### Usage
* Run dotnet run to start the application.
* Connect to the API using Postman or swagger on port 5016.

### API Endpoints
| HTTP Verbs | Endpoints | Action |
| --- | --- | --- |
| GET | /api/SwitchBoard | Get all container status |
| POST | /api/SwitchBoard | Starts a container |
| DELETE | /api/SwitchBoard | Stops and removes a container |

### Api Documet
{
  "openapi": "3.0.1",
  "info": {
    "title": "SwitchBoardApi",
    "version": "1.0"
  },
  "paths": {
    "/api/SwitchBoard": {
      "get": {
        "tags": [
          "SwitchBoardApi"
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContainerCondition"
                  }
                }
              },
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContainerCondition"
                  }
                }
              },
              "text/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "$ref": "#/components/schemas/ContainerCondition"
                  }
                }
              }
            }
          }
        }
      },
      "post": {
        "tags": [
          "SwitchBoardApi"
        ],
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/ContainerRequest"
              }
            },
            "text/json": {
              "schema": {
                "$ref": "#/components/schemas/ContainerRequest"
              }
            },
            "application/*+json": {
              "schema": {
                "$ref": "#/components/schemas/ContainerRequest"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Success"
          }
        }
      },
      "delete": {
        "tags": [
          "SwitchBoardApi"
        ],
        "parameters": [
          {
            "name": "containerId",
            "in": "query",
            "style": "form",
            "schema": {
              "type": "string"
            }
          }
        ],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "text/plain": {
                "schema": {
                  "type": "string"
                }
              },
              "application/json": {
                "schema": {
                  "type": "string"
                }
              },
              "text/json": {
                "schema": {
                  "type": "string"
                }
              }
            }
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "ContainerCondition": {
        "type": "object",
        "properties": {
          "containerId": {
            "type": "string",
            "nullable": true
          },
          "status": {
            "type": "string",
            "nullable": true
          },
          "containerName": {
            "type": "string",
            "nullable": true
          }
        },
        "additionalProperties": false
      },
      "ContainerRequest": {
        "required": [
          "containerName",
          "image"
        ],
        "type": "object",
        "properties": {
          "image": {
            "minLength": 1,
            "type": "string"
          },
          "containerName": {
            "minLength": 1,
            "type": "string"
          },
          "mountSource": {
            "type": "string",
            "nullable": true
          },
          "mountTarget": {
            "type": "string",
            "nullable": true
          },
          "enviorment": {
            "type": "array",
            "items": {
              "type": "string"
            },
            "nullable": true
          }
        },
        "additionalProperties": false
      }
    }
  }
}
