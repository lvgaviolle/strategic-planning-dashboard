# Northstar · Strategic Planning Dashboard

> **Portfolio Edition** — aplicação demonstrativa e independente construída em Blazor Web App (.NET 9).

O **Northstar Planning Studio** é um painel de metas e indicadores com foco em clareza visual e regras de acompanhamento. O objetivo é apresentar habilidades de desenvolvimento full stack de forma segura, usando **somente código criado para este portfólio e dados inventados**.

## Funcionalidades

- Dashboard responsivo com cartões de desempenho e acompanhamento de metas.
- Filtros combinados por unidade, área e busca textual.
- Comparação de previsto e realizado com barras de progresso.
- Classificação automática: **atingida** (>= 100%), **em atenção** (>= 80%) e **abaixo da meta** (< 80%).
- Cadastro de metas com validação de formulário.
- Edição de realizado e inativação/reativação de metas.
- Restauração do conjunto fictício de exemplo.
- Interface em português brasileiro com formatação de moeda, percentual e quantidade.

## Tecnologias

| Camada | Tecnologia |
| --- | --- |
| UI | Razor Components, Blazor Interactive Server, HTML, CSS |
| Backend | C# / ASP.NET Core 9 |
| Estado | Serviço Scoped com armazenamento em memória |
| Qualidade | GitHub Actions — restore e build |

**Não há conexão com banco de dados, API financeira, serviços externos ou sistemas empresariais.** Os dados são descartáveis e reiniciados em uma nova sessão.

## Executar localmente

Pré-requisito: **SDK .NET 9** instalado.

~~~bash
git clone https://github.com/lvgaviolle/strategic-planning-dashboard.git
cd strategic-planning-dashboard
dotnet restore
dotnet run
~~~

Abra no navegador a URL local exibida no terminal (normalmente http://localhost:5000 ou a porta configurada pelo .NET).

## Organização

~~~text
Components/
  Layout/            Navegação e estrutura visual
  Pages/             Dashboard e cadastro/gestão
Models/               Entidades e validações
Services/             Dados de demonstração e operações
wwwroot/              Estilos
.github/workflows/    Build automatizado
~~~

## Limitações intencionais

- Sem autenticação: qualquer execução local é uma demonstração.
- Sem persistência: alterações são temporárias e ficam no circuito/sessão do Blazor Server.
- A primeira versão acompanha o mês corrente; os indicadores usam a lógica **quanto maior, melhor**.
- Não há cálculo consolidado de percentuais entre unidades: cada indicador é apresentado individualmente.
- Esta edição **não reproduz interfaces, banco, código, marcas, dados ou regras proprietárias** de projetos profissionais.

## English summary

A standalone **Blazor .NET 9 interactive dashboard** featuring goal management, multi-dimensional filters, KPI progress visualization, validation, and in-memory demo data. Built from scratch as a safe technical portfolio sample, with no proprietary company code or information.

## Próximos passos

- Testes automatizados de regras de metas.
- Persistência opcional em SQLite.
- Autenticação demonstrativa com perfis.
- Histórico mensal e comparativos.
- Capturas de tela e publicação de uma demonstração acessível.

---

**Status:** MVP demonstrativo, primeira versão. Publicação pública somente após revisão final do autor.
