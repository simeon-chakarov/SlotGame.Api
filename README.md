# Slot Game API

## Overview
This is a slot game engine implemented as a REST API using ASP.NET Core.

The system allows:
- Creating games with custom reel strips
- Executing spins with cascading mechanics
- Retrieving spin results and history

---

## Tech Stack
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- SQLite
- FluentValidation
- Swagger (OpenAPI)

---

## Getting Started

1. Clone the repository

2. Open the solution in Visual Studio

3. Run the application (F5)

The database will be created automatically on startup via EF Core migrations.

4. Open Swagger: https://localhost:<your-port>/swagger

---

## Database

- SQLite is used for persistence
- The database file is **not included in the repository**
- It is automatically created on startup using EF Core migrations

---

## Notes

- All endpoints are prefixed with `/api` following standard REST conventions
- EF Core is used directly without repository pattern to keep the solution simple and focused
- The project is designed to be easy to run with minimal setup

---

## API Endpoints

### Games
- `POST /api/game` – Create a new game
- `GET /api/games` – Get all games
- `GET /api/games/{id}` – Get game by id

### Engine
- `POST /api/engine` – Execute a spin
- `GET /api/engine/{spinId}` – Get spin result by id

### History
- `GET /api/history?spinsPerPage=10&pageNumber=1`

---

## Game Logic

- The game uses an 8x8 matrix generated from 8 reel strips (circular arrays)
- Each column is derived from a reel using a random starting index

### Winning
- A symbol wins if it appears 8 or more times in the matrix
- Symbol `0` is not a winning symbol
- Payout per symbol is: `symbolValue * betAmount`

### Cascading
- Winning symbols are removed
- Remaining symbols fall down (gravity)
- New symbols are added from the reels (reverse scrolling)
- Cascades continue until no win or a maximum of 10 cascades is reached
