# Northstar · Strategic Planning Dashboard

> **Portfolio Edition** — painel demonstrativo independente em **C# / Blazor Web App (.NET 9)**, com persistência SQLite, histórico mensal e testes automatizados.

## Sobre o projeto

O **Northstar Planning Studio** é uma aplicação de portfólio desenvolvida de forma independente para demonstrar arquitetura .NET, componentes interativos, persistência, validação e visualização de metas. **Todas as unidades, metas, nomes e métricas são fictícios.** Não contém código, dados, marcas, credenciais, esquemas ou integrações de empregadores.

## Funcionalidades

- Dashboard responsivo, em português brasileiro, com indicadores de desempenho.
- Filtros por período, unidade, área e busca textual.
- Histórico de seis meses com visualização gráfica de atingimento médio.
- Cadastro de metas e atualização do realizado com persistência SQLite.
- Ativação, inativação e restauração confirmada dos dados fictícios.
- Regras isoladas de cálculo de progresso e testes unitários.
- GitHub Actions para build e testes.

**Regra de cálculo:** cada meta possui seu próprio previsto e realizado. O progresso individual é realizado / previsto × 100; no indicador histórico agregado, cada progresso é limitado a 100% e todos possuem pesos iguais. Valores em reais, quantidades e percentuais não são somados indiscriminadamente. O escopo demonstrativo assume que maior realizado significa melhor resultado.

## Executar localmente

Pré-requisito: **SDK .NET 9**.

~~~powershell
dotnet restore StrategicPlanningDashboard.csproj
dotnet run --project StrategicPlanningDashboard.csproj
~~~

O terminal informa a URL do servidor, geralmente **http://localhost:5000**. O banco SQLite é criado automaticamente em `App_Data/northstar.db` e recebe dados de exemplo na primeira execução.

Para rodar os testes:

~~~powershell
dotnet restore tests/StrategicPlanningDashboard.Tests/StrategicPlanningDashboard.Tests.csproj
dotnet test tests/StrategicPlanningDashboard.Tests/StrategicPlanningDashboard.Tests.csproj
~~~

O banco **não deve ser enviado ao GitHub** (o `.gitignore` exclui a pasta `App_Data/`).

### Se você usou Download ZIP

1. Interrompa a aplicação antiga no PowerShell com **Ctrl+C**.
2. Baixe um novo ZIP da branch `main` em **Code → Download ZIP**.
3. Extraia e abra a pasta atualizada.
4. Execute os comandos `dotnet restore` e `dotnet run` acima.

O ZIP antigo usava dados em memória e **não possuía banco SQLite**. Ele não contém alterações persistidas para migrar.

## Estrutura

~~~text
Components/
  Layout/          Navegação e layout
  Pages/           Visão geral e gestão de metas
Models/            Entidades e validações
Services/
  PlanningStore.cs SQLite local e dados sintéticos
  GoalMetrics.cs   Consolidação normalizada
tests/             Testes de regra e persistência
wwwroot/           Estilos responsivos
.github/workflows/ Pipeline CI
~~~

## Aviso de segurança e demonstração pública

**Esta versão é para execução local e ambientes controlados.** Não existe autenticação e as alterações são feitas em um banco compartilhado por todo o processo. **Não exponha diretamente o aplicativo na internet:** visitantes poderiam modificar ou restaurar os dados. Antes de implantar uma demo pública é necessário implementar acesso somente leitura ou sessões isoladas, limitar recursos e definir persistência adequada ao provedor.

Para mudar o caminho do banco, defina a variável de ambiente `NORTHSTAR_DB_PATH` antes de executar. Em ambientes de hospedagem, certifique-se de utilizar volume persistente se desejar preservar o banco.

## Qualidade e próximos passos

O workflow de GitHub Actions compila a aplicação e roda testes de regra e integração com SQLite. O resultado de CI deve ser conferido na aba **Actions**; a presença do workflow não garante que uma execução tenha sido concluída.

Próximos incrementos planejados: comparação por indicador entre meses, documentação com screenshots e preparação de uma demo pública segura.

## English overview

An independent **Blazor .NET 9** portfolio app featuring an interactive KPI dashboard, monthly history, SQLite persistence, business rules, responsive UI and automated tests. Contains synthetic demo data only, with no proprietary code or company information.
