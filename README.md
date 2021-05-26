# RestaurantSelectorService
 
API REST Service to help users decide the restaurant where they will eat each day by voting

## Compila��o

Precisa de um sistema de consiga compilar aplica��o .net Core 5

## Execu��o

Executar o comando abaixo(acessando a pasta onde o projeto foi descompactado) para iniciar o servi�o:

dotnet run

O servi�o dever� iniciar na porta 5001. A solu��o tamb�m pode ser aberta e executada via Visual Studio, com o n�mero da porta variando.
E o comando abaixo para executar todos os testes automatizados:

dotnet test

## Dados

Esse exemplo n�o usa banco de dados. Cada vez que � aberto o sistema, ele cria 50 usu�rios e 10 restaurantes, que ficam guardados em mem�ria.

## Rotas

Rota para inserir um voto no restaurante escolhido do dia
POST - https://localhost:5001/api/vote/
{
	"userId": int,
    "restaurantId": int
}

Rota para retornar o restaurante escolhido do dia. Se um for dado como vitorioso, o dia � passado automaticamente.
GET - https://localhost:5001/api/vote/result

Rota para reiniciar os dados do servi�o
GET - https://localhost:5001/api/vote/reset

## O que vale destacar no c�digo implementado?

Nada demais, � apenas uma API REST.

## O que poderia ser feito para melhorar o sistema?

- Adicionar um banco de dados;
- Realocar as mensagens de texto em uma classe que permitisse localiza��o;
- Criar rotas de cria��o de Usu�rio e Restaurantes;
- Padronizar os nomes de testes;

## Algo a mais que voc� tenha a dizer

Eu acabei fazendo bem simples e r�pido o sistema. Pelo que vi, atendi todos os requisitos pedidos. Espero n�o ser descontado por ter usado aquelas listas estaticas, apenas quis simplificar e focar nas rotas e valida��es.