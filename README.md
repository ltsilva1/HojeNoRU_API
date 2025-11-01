# HojeNoRU API 🍽️

API REST em ASP.NET Core que realiza *web scraping* do cardápio semanal dos Restaurantes Universitários da UFRGS.

---

## Tecnologias envolvidas
- C# / .NET 9
- ASP.NET Core Web API
- Entity Framework Core + SQLite

---

## Endpoints atuais
| Endpoint | Descrição |
|-----------|------------|
| `/api/refeicoes` | Lista todas as refeições |
| `/api/refeicoes/ru/{id}` | Lista refeições de um RU específico (ex.: 06) |
| `/api/refeicoes/dia/{diaSemana}` | Lista refeições de um dia da semana (ex.: "quarta-feira") |
| `/api/refeicoes/dia/{diaSemana}/tipo/{tipo}` | Filtra por dia da semana e tipo (almoço/jantar) |
| `/api/refeicoes/atualizar` | Atualiza o banco via scraping |

---

## Execução local
```bash
git clone https://github.com/ltsilva1/HojeNoRU_API.git
cd HojeNoRU_API/HojeNoRU_API
dotnet restore
dotnet ef migrations add CriacaoInicial
dotnet ef database update
dotnet run
