
# Проект Kafka

Этот проект демонстрирует работу с Kafka, используя C# и библиотеку Confluent.Kafka. Он состоит из трех отдельных приложений: Producer, Pull Consumer и Push Consumer.

### KafkaProducer

Производитель сообщений для Kafka.

### KafkaPullConsumer

Потребитель сообщений из Kafka, использующий метод pull.

### KafkaPushConsumer

Потребитель сообщений из Kafka, использующий метод push.

## Настройка и запуск

1. Убедитесь, что у вас установлен .NET SDK.
2. Клонируйте этот репозиторий на вашу локальную машину.
3. Перейдите в директорию проекта в терминале.
4. Запустите Kafka с помощью Docker:
   ```
   docker-compose up -d
   ```
5. Для запуска каждого приложения используйте команду:
   ```
   dotnet run
   ```
   Выполните эту команду в директории каждого приложения (KafkaProducer, KafkaPullConsumer, KafkaPushConsumer).

## Проверка работы приложений

1. Запустите KafkaProducer:
   ```
   cd KafkaProducer
   dotnet run
   ```
   Вы должны увидеть сообщения о успешной отправке в консоли.

2. Запустите KafkaPullConsumer:
   ```
   cd KafkaPullConsumer
   dotnet run
   ```
   Вы должны увидеть сообщения, полученные из Kafka.

3. Запустите KafkaPushConsumer:
   ```
   cd KafkaPushConsumer
   dotnet run
   ```
   Вы должны увидеть сообщения, автоматически получаемые из Kafka.


http://localhost:8080 интерфейс для управления Kafka.
