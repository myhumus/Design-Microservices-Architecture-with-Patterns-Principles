# Configurazione Postgres con Aspire - File .env

Questa soluzione utilizza un file `.env` per gestire le credenziali di Postgres in modo sicuro.

## Come funziona

1. **File `.env`**: Contiene le variabili di ambiente con username e password di Postgres
2. **Program.cs**: Carica automaticamente le variabili dal file `.env` all'avvio dell'AppHost
3. **Aspire**: Usa le credenziali per creare il container PostgreSQL

## Setup

### Passo 1: Configurare il file `.env`

Modifica il file `AppHost/.env` con le tue credenziali:

```env
POSTGRES_USERNAME=postgres
POSTGRES_PASSWORD=your_secure_password_here
```

> ⚠️ **Importante**: Questo file contiene dati sensibili. Non eseguire il commit in Git!
> Il file `.gitignore` è già configurato per escludere `.env`.

### Passo 2: Usare le credenziali

Quando avvii l'AppHost:

```bash
dotnet run --project AppHost
```

L'applicazione:
1. Legge il file `.env`
2. Carica le variabili di ambiente
3. Crea il container PostgreSQL con le credenziali specificate
4. Avvia pgAdmin sulla porta http://localhost:5050 (se configurato)

## Struttura del codice

```csharp
// Caricamento dal file .env
var envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFilePath))
{
	foreach (var line in File.ReadAllLines(envFilePath))
	{
		// Parsing e caricamento in Environment
	}
}

// Lettura delle credenziali
var postgresUsername = Environment.GetEnvironmentVariable("POSTGRES_USERNAME") ?? "postgres";
var postgresPassword = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? 
	throw new InvalidOperationException("...non trovata...");

// Creazione dei parametri per Aspire
var usernameParam = builder.AddParameter("postgres-username", postgresUsername, secret: false);
var passwordParam = builder.AddParameter("postgres-password", postgresPassword, secret: true);

// Configurazione di Postgres con le credenziali
var postgres = builder.AddPostgres("postgres", usernameParam, passwordParam);
```

## Integrazione Aspire

La soluzione utilizza le seguenti integrazioni Aspire:

- **Aspire.Hosting.PostgreSQL**: Hosting PostgreSQL
- **pgAdmin**: Strumento di amministrazione web per PostgreSQL
- **Data Volume**: Persistenza dei dati tra i riavvii

## Riferimenti

- [Documentazione Aspire - PostgreSQL Host](https://aspire.dev/integrations/databases/postgres/postgres-host/)
- [Documentazione Aspire - PostgreSQL Connect](https://aspire.dev/integrations/databases/postgres/postgres-connect/)
- [pgAdmin Documentation](https://www.pgadmin.org/)

## Sicurezza

- Il file `.env` non è tracciato da Git grazie al `.gitignore`
- La password è segnata come `secret: true` nell'Aspire Dashboard
- Usa password robuste in produzione
- Considera l'uso di Azure Key Vault per ambienti production
