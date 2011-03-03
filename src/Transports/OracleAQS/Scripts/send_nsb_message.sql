create or replace
PROCEDURE SEND_NSB_MESSAGE 
(
  target_service IN VARCHAR2  
, message_name IN VARCHAR2  
, message_namespace IN VARCHAR2
, message_content IN VARCHAR2  
) AS 

  messageContract CONSTANT VARCHAR2(200) := 'NServiceBusTransportMessageContract';
  messageType CONSTANT VARCHAR2(200) := 'NServiceBusTransportMessage';
  transportXml SYS.XMLType;
  transportMessage VARCHAR2(4000);
  messageId RAW(16);
  enqueueOptions DBMS_AQ.ENQUEUE_OPTIONS_T;
  messageProperties DBMS_AQ.MESSAGE_PROPERTIES_T;
  
BEGIN
  
  transportMessage := '<?xml version="1.0"?><TransportMessage><Body><![CDATA[<Messages xmlns="http://tempuri.net/'
  || message_namespace || '"><'
  || message_name || '>'
  || message_content 
  || '</' || message_name || '></Messages>]]></Body></TransportMessage>';

  transportXml := SYS.XMLType.createXML(transportMessage);
  enqueueOptions.visibility := DBMS_AQ.ON_COMMIT;
  
  DBMS_AQ.ENQUEUE( queue_name => target_service, 
  enqueue_options => enqueueOptions,
  message_properties => messageProperties,
  payload => transportXml, 
  msgid => messageId );
  
  COMMIT;
END SEND_NSB_MESSAGE;