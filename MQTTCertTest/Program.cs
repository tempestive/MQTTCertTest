using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;

using System.Security.Cryptography.X509Certificates;


// get command line arguments
int i = 0;
string broker = args[i++];
string userid = args[i++];
string password = args[i++];
string expirationDateAsString = args[i++];

DateTime expirationDateToCheck = DateTime.Parse(expirationDateAsString);
Random rand = new Random();
string clientId = $"TCERT-{rand.Next(1000000)}";
bool isExpired = true;
MqttFactory mqttFactory = new MqttFactory();
IMqttClient? mqttClient = mqttFactory.CreateMqttClient();

/// <summary>
/// Callback to validate the certificate
/// </summary>
bool ValidateCert(MqttClientCertificateValidationCallbackContext arg)
{
    X509Certificate? cert = arg.Certificate;
    Console.WriteLine(cert.Subject);
    Console.WriteLine(cert.GetExpirationDateString());
    DateTime certExpirationDate = DateTime.Parse(cert.GetExpirationDateString());
    isExpired = certExpirationDate < expirationDateToCheck;
    return true;
}

/// <summary>
/// Get build options for TLS
/// </summary>
MqttClientOptionsBuilderTlsParameters GetBuildTlsOptions()
{
    MqttClientOptionsBuilderTlsParameters p = new MqttClientOptionsBuilderTlsParameters();
    p.UseTls = true;
    p.CertificateValidationHandler = ValidateCert;
    p.IgnoreCertificateRevocationErrors = true;    
    return p;
}

/// <summary>
/// Create connect options for MQTT
/// </summary>
IMqttClientOptions? connectOptions = (new MqttClientOptionsBuilder())
    .WithTls(GetBuildTlsOptions())
    .WithCredentials(userid, password)
    .WithTcpServer(broker)
    .WithClientId(clientId)
    //.WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V310)
    //.WithCommunicationTimeout(TimeSpan.FromMinutes(30))
    //.WithKeepAlivePeriod(TimeSpan.FromMilliseconds(1500))
    //.WithKeepAliveSendInterval(TimeSpan.FromMilliseconds(200))
    .Build();

///// <summary>
///// Create connect options for MQTT
///// </summary>
//IMqttClientOptions? connectOptions = (new MqttClientOptionsBuilder())
//    .WithTls(GetBuildTlsOptions())
//    .WithWebSocketServer(broker)
//    .WithCredentials(userid, password)
//    //.WithTcpServer(broker)
//    .WithClientId(clientId)
//    //.WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V310)
//    //.WithCommunicationTimeout(TimeSpan.FromMinutes(30))
//    //.WithKeepAlivePeriod(TimeSpan.FromMilliseconds(1500))
//    //.WithKeepAliveSendInterval(TimeSpan.FromMilliseconds(200))    
//    .Build();

// main program
CancellationToken cancellationToken = CancellationToken.None;
MqttClientConnectResult? connectionResult = await mqttClient.ConnectAsync(connectOptions, cancellationToken);
await mqttClient.PingAsync(cancellationToken);
Console.WriteLine($"Connect result: {connectionResult?.ResultCode}");
await mqttClient.DisconnectAsync(cancellationToken);
Console.WriteLine($"Cert is expired: {isExpired}");

// return 0 if not expired - useful for shell scripts
return isExpired ? -1 : 0;

