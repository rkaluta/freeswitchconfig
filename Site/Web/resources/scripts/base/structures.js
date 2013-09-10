function sDomainExtensionPair(extension, domain) {
    this.Extension = extension;
    this.Domain = domain;
}

function sGatewayNumberPair(number, gatewayName) {
    this.Number = number;
    this.GatewayName = gatewayName;
}

function sExtensionContextPair(extension, context) {
    this.Extension = extension;
    this.Context = context;
}

function sAdvancedPin(extension,domain,pin)
{
    this.Extension = extension;
    this.Domain=domain;
    this.Pin = pin;
}