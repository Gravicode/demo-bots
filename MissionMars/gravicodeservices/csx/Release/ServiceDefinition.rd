<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="gravicodeservices" generation="1" functional="0" release="0" Id="4953ebce-37ad-44c4-a347-c5e881ba1e3c" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="gravicodeservicesGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="IoTBroker:mqtt_http" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/gravicodeservices/gravicodeservicesGroup/LB:IoTBroker:mqtt_http" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="IoTBroker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/gravicodeservices/gravicodeservicesGroup/MapIoTBroker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="IoTBrokerInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/gravicodeservices/gravicodeservicesGroup/MapIoTBrokerInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:IoTBroker:mqtt_http">
          <toPorts>
            <inPortMoniker name="/gravicodeservices/gravicodeservicesGroup/IoTBroker/mqtt_http" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapIoTBroker:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/gravicodeservices/gravicodeservicesGroup/IoTBroker/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapIoTBrokerInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/gravicodeservices/gravicodeservicesGroup/IoTBrokerInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="IoTBroker" generation="1" functional="0" release="0" software="C:\Experiment\MissionMars\gravicodeservices\csx\Release\roles\IoTBroker" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="mqtt_http" protocol="tcp" portRanges="1883" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;IoTBroker&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;IoTBroker&quot;&gt;&lt;e name=&quot;mqtt_http&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/gravicodeservices/gravicodeservicesGroup/IoTBrokerInstances" />
            <sCSPolicyUpdateDomainMoniker name="/gravicodeservices/gravicodeservicesGroup/IoTBrokerUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/gravicodeservices/gravicodeservicesGroup/IoTBrokerFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="IoTBrokerUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="IoTBrokerFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="IoTBrokerInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="4eada32f-8c77-4681-ae93-f2f4f3a5d611" ref="Microsoft.RedDog.Contract\ServiceContract\gravicodeservicesContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="9f20f598-a081-479c-9919-bb825d2f7c4f" ref="Microsoft.RedDog.Contract\Interface\IoTBroker:mqtt_http@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/gravicodeservices/gravicodeservicesGroup/IoTBroker:mqtt_http" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>