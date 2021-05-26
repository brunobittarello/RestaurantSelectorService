# RestaurantSelectorService
 
API REST Service to help users decide the restaurant where they will eat each day by voting

## Compilação

Precisa de um sistema de consiga compilar aplicação .net Core 5

## Execução

Executar o comando abaixo(acessando a pasta onde o projeto foi descompactado) para iniciar o serviço:

dotnet run

O serviço deverá iniciar na porta 5001. A solução também pode ser aberta e executada via Visual Studio, com o número da porta variando.
E o comando abaixo para executar todos os testes automatizados:

dotnet test

## Dados

Esse exemplo não usa banco de dados. Cada vez que é aberto o sistema, ele cria 50 usuários e 10 restaurantes, que ficam guardados em memória.

## Rotas

Rota para inserir um voto no restaurante escolhido do dia
POST - https://localhost:5001/api/vote/
{
	"userId": int,
    "restaurantId": int
}

Rota para retornar o restaurante escolhido do dia. Se um for dado como vitorioso, o dia é passado automaticamente.
GET - https://localhost:5001/api/vote/result

Rota para reiniciar os dados do serviço
GET - https://localhost:5001/api/vote/reset

## O que vale destacar no código implementado?

Nada demais, é apenas uma API REST.

## O que poderia ser feito para melhorar o sistema?

- Adicionar um banco de dados;
- Realocar as mensagens de texto em uma classe que permitisse localização;
- Criar rotas de criação de Usuário e Restaurantes;
- Padronizar os nomes de testes;

## Algo a mais que você tenha a dizer

Eu acabei fazendo bem simples e rápido o sistema. Pelo que vi, atendi todos os requisitos pedidos. Espero não ser descontado por ter usado aquelas listas estaticas, apenas quis simplificar e focar nas rotas e validações.