-- create the queues
execute dbms_aqadm.create_queue_table(queue_table=>'hr.test_q_tab',queue_payload_type=>'SYS.XMLType',multiple_consumers=>FALSE);
execute dbms_aqadm.create_queue(queue_name=>'hr.test_q',queue_table=>'hr.test_q_tab');
execute dbms_aqadm.start_queue(queue_name=>'hr.test_q');

-- grant HR permission to interact with the queues
GRANT EXECUTE ON dbms_aqadm TO HR;
GRANT EXECUTE ON dbms_aq TO HR;

-- delete the queues
execute DBMS_AQADM.STOP_QUEUE('hr.test_q');
execute DBMS_AQADM.DROP_QUEUE(  queue_name => 'hr.test_q',  auto_commit => TRUE);
execute DBMS_AQADM.DROP_QUEUE_TABLE(  queue_table => 'hr.test_q_tab',   force => FALSE,     auto_commit => TRUE);
