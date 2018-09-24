// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the WLPINTERFACE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// WLPINTERFACE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef WLPINTERFACE_EXPORTS
#define WLPINTERFACE_API __declspec(dllexport)
#else
#define WLPINTERFACE_API __declspec(dllimport)
#endif


#define PORTA ((unsigned char)0)
#define PORTB ((unsigned char)1)
#define PORTC ((unsigned char)2)

#define DIRECTION_TX (0)
#define DIRECTION_RX (1)

#define WLP_FAIL 0
#define WLP_SUCCESS 1

enum EGpioStatus
{ 
	GPIOSTATUS_OUTPUT = 0,
	GPIOSTATUS_INPUT,
	GPIOSTATUS_NA
};

enum EGpioValue
{
	GPIOVALUE_CLEARED = 0,
	GPIOVALUE_SET
};

enum EGpioConfig
{
	GPIOCONFIG_INPUT_FLOATING = 0,
	GPIOCONFIG_INPUT_PULL_UP,
	GPIOCONFIG_INPUT_PULL_DOWN,
	GPIOCONFIG_OUTPUT_OPEN_DRAIN_NO_PULL,
	GPIOCONFIG_OUTPUT_OPEN_DRAIN_PULL_UP,
	GPIOCONFIG_OUTPUT_PUSH_PULL
};

enum EGpioConfigStatus
{
	GPIOCONFIGSTATUS_OK = 0,
	GPIOCONFIGSTATUS_NOT_SUPPORTED,
	GPIOCONFIGSTATUS_NA
};

enum EGpioOutputStatus
{
	GPIOOUTPUTSTATUS_OK = 0,
	GPIOOUTPUTSTATUS_PIN_NOT_OUTPUT,
	GPIOOUTPUTSTATUS_NA
};

enum ECsrMicConfig
{
	CSRMICCONFIG_NONE = 0,
	CSRMICCONFIG_MIC1,
	CSRMICCONFIG_MIC2,
	CSRMICCONFIG_BOTH,
	CSRMICCONFIG_MICCVC
};

enum ECsrPsTransportConfig
{
	CSRPSTRANS_UART = 0,
	CSRPSTRANS_SPI
};

enum ECsrPsSetGetConfig
{
	CSRPS_SET = 0,
	CSRPS_GET
};

// When building for system test, the client application will be built with gcc.
// Due to name mangling differences between compilers, a C++ library built by one compiler
// can only be consumed by another compiler if it uses standard C calling convention.
#ifdef SYSTEM_TEST
extern "C" {
#endif

//-----------------------------------------------------------------------------------------
// Initialize the DLL to prepare for using the functions in the WLPInterface API.
// This must be done before calling any other functions in the DLL.
//
// \param pSettingsDefinition		Filepath to settings definition file for the device.
//									Typical "SettingsDefinitionLeyline.xml"
// \return							Error code from the initialization function
//									0 If initialization failed
//									1 If initialization succeeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPInit(const char* pSettingsDefinitionPath);

//-----------------------------------------------------------------------------------------
// Close down DLL and release all resources. Should be called as the very last call
// before exiting the program.
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API void WLPExit();

//-----------------------------------------------------------------------------------------
// Return the error message from the last call to a device command
//
// \Handle				Handle to device
// \Message				Pointer to array provided by caller, this will contain the returned
//                      error message
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API void WLPLastError(int Handle, char Message[256]);

//-----------------------------------------------------------------------------------------
// Return the global error message from the last API call. These messages are errors
// that cannot be traced to a specific handle
//
// \Message				Pointer to array provided by caller, this will contain the returned
//                      error message
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API void WLPGlobalError(char Message[256]);

//-----------------------------------------------------------------------------------------
// Open a connection to a new Leyline device
//
// \return				Handle
//                      0 if failed to open a new device
//						<>0 if found a device. Use this value to communicate with device
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPConnect();

//-----------------------------------------------------------------------------------------
// Disconnect from Leyline device
//
// \param Handle		Handle to attached device obtained from a previous connect command
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API void WLPDisconnect(int Handle);

//-----------------------------------------------------------------------------------------
// Retrieve the version of the firmware in the attached device
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Version		Array with the returned firmware version of the attached device
//                      Example: "1.0.0"
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPInfoVersion(int Handle, char Version[16]);

//-----------------------------------------------------------------------------------------
// Retrieve the image mode of the attached device
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param pMode			Pointer to returned image mode code (pointer to single element). 
//                      0 Loader image
//                      1 Application image
//                      2 Service image
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPInfoMode(int Handle, unsigned char* pMode);

//-----------------------------------------------------------------------------------------
// Read the GPIO pin value of the specified GPIO input pin
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Port			GPIO port on Microcontroller
//                      0 PORTA
//                      1 PORTB
//                      2 PORTC
// \param Pin			GPIO pin on the specified port
// \param pGpioStatus   Pointer to returned Gpio status defined by the EGpioStatus enum
// \param pGpioValue	Pointer to returned Gpio value defined by EGpioValue
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPGpioReadPin(int Handle, unsigned char Port, unsigned char Pin, int* pGpioStatus, int* pGpioValue);

//-----------------------------------------------------------------------------------------
// Clear the specified GPIO output pin to LOW
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Port			GPIO port on Microcontroller
//                      0 PORTA
//                      1 PORTB
//                      2 PORTC
// \param Pin			GPIO pin on the specified port
// \param pGpioOutputStatus Pointer to returned Gpio output status defined by EGpioOutputStatus
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPGpioClearOutputPin(int Handle, unsigned char Port, unsigned char Pin, int* pGpioOutputStatus);

//-----------------------------------------------------------------------------------------
// Set the specified GPIO output pin to HIGH
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Port			GPIO port on Microcontroller
//                      0 PORTA
//                      1 PORTB
//                      2 PORTC
// \param Pin			GPIO pin on the specified port
// \param pGpioOutputStatus Pointer to returned Gpio output status defined by EGpioOutputStatus
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPGpioSetOutputPin(int Handle, unsigned char Port, unsigned char Pin, int* pGpioOutputStatus);

//-----------------------------------------------------------------------------------------
// Configure specified GPIO pin to input or output
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Port			GPIO port on Microcontroller
//                      0 PORTA
//                      1 PORTB
//                      2 PORTC
// \param Pin			GPIO pin on the specified port
// \param GpioConfig    Desired GPIO config value defined by EGpioConfig enum
// \param pGpioConfigStatus	Pointer to returned Gpio config status defined by EGpioConfigStatus enum
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPGpioConfigurePin(int Handle, unsigned char Port, unsigned char Pin, int GpioConfig, int* pGpioConfigStatus);

//-----------------------------------------------------------------------------------------
// Set PS-Key in Bluetooth BC5-MM
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Address		PSKey address in CSR BC5-MM
// \param pValues		Array of PSKey values to set
// \param Length		Length of array to write (number of items in array)
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPCsrPsSet(int Handle, int Address, const int* pValues, int Length);

//-----------------------------------------------------------------------------------------
// Read PS-Key from Bluetooth BC5-MM
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Address		PSKey address in CSR BC5-MM
// \param Values		Pointer to array where the read values must be copied to
//                      The array buffer must be provided by the caller
// \param pLength		Pointer to returned settings length (number of items in the Values array)
// 
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPCsrPsGet(int Handle, int Address, int pValues[1024], int* pLength);

//-----------------------------------------------------------------------------------------
// Read or Write PS-Key from Bluetooth BC5-MM via Selectable Transports
//
// \param Handle				Handle to attached device obtained from a previous connect command
// \param CsrPsTransportConfig	Parameter to denote the Transport to the CSR BlueCore to use, defined by ECsrPsTransportConfig
//								Examples of setups to select transport could be:
//								PC <-USB-> ARM Controller <-UART-> CSR BlueCore	-  To use this path you would select UART Transport to the BlueCore
//								PC <-SPI-> CSR BlueCore							-  To use this path you would select SPI Transport to the BlueCore
//
// \param CsrPsSetGetConfig		Parameter which controls whether to Set or Get the PSKey at 'Address' - Parameter is defined by ECsrPsSetGetConfig
// \param Address				PSKey address in CSR BC5-MM
// \param pValues				Pointer to array where the read values must be copied to on a Get
//								Pointer to array where the written values must be copied from on a Set
//								The array buffer must be provided by the caller
// \param Length				Returned settings length (number of items in the Values array) on a Get
//								Number of items in the Values array to write on a Set
// 
// 
// \return						Connection return code. 
//								0 if communication connection failed
//								1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPCsrPsSetGet(int Handle, int CsrPsTransportConfig, int CsrPsSetGetConfig, int Address, int* pValues, int* pLength);

//-----------------------------------------------------------------------------------------
//
// \param Handle		Handle to attached device obtained from a previous connect command
// 
// \return				Connection return code. 
//                      0 if communication connection failed (Clear Pairings Failed)
//                      1 if communication succeded (Clear Pairings Succeeded)
// Function
// Inclusive Time		A successfull execution takes approx. 1 second
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPCsrClearPairings(int Handle);

//-----------------------------------------------------------------------------------------
//
// \param Handle		Handle to attached device obtained from a previous connect command
// 
// \return				The measured RSSI value if measure succeeded
//						otherwise the function returns error code [-300] through [-399]
//
// Function
// Inclusive Time		A successful execution takes approx. 5 second
//-----------------------------------------------------------------------------------------
//--------------------- Possible error codes returned -------------------------------------
/*
-301	hci_error_illegal_command					Indicates that the controller does not understand the HCI Command      
													Packet OpCode that the host sent. The OpCode given might not correspond  
													to any of the OpCodes specified in this document,																																															
													OpCodes, or the command may not have been implemented.																																															
	                              																																																	
-302	hci_error_no_connection						Indicates that a command was sent from the host that should identify a 
													connection, but that connection does not exist.																																															
	                              																																																	
-303	hci_error_hardware_fail						Indicates to the host that something in the controller has failed in a 
													manner that cannot be described with any other error code.																																															
	                              																																																	
-304	hci_error_page_timeout						Indicates that a page timed out because of the Page Timeout 																																															           
													configuration parameter. This error code may occur only with																																															 
													the HCI_Remote_Name_Request and HCI_Create_Connection commands. 
	                              																																																	
-305	hci_error_auth_fail							Indicates that pairing or authentication failed due to incorrect       
													results in the pairing or authentication procedure. This could be due to 
													an incorrect PIN or Link Key.																																															
	                              																																																	
-306	hci_error_key_missing						Used when pairing failed because of a missing PIN.																																															
	                              																																																	
-307	hci_error_memory_full						Indicates to the host that the controller has run out of memory to     
													store new parameters.																																															
	                              																																																	
-308	hci_error_conn_timeout						Indicates that the link supervision timeout has expired for a given    
													connection.																																															
	                              																																																	
-309	hci_error_max_nr_of_conns					Indicates that an attempt to create another connection failed because  
													the controller is already at its limit of the number of connections it   
													can support.																																															
	                              																																																	
-310	hci_error_max_nr_of_sco						Indicates that the controller has reached the limit to the number of   
													synchronous connections that can be achieved to a device. 																																															
	                              																																																	
-311	hci_error_max_nr_of_acl						Indicates that the controller has reached the limit to the number of   
													asynchronous connections that can be achieved to a device. 																																															
	                              																																																	
-312	hci_error_command_disallowed				Indicates that the command requested cannot be executed because the    
													controller is in a state where it cannot process this command at this    
													time. This error shall not be used for command OpCodes where the error   
													code Unknown HCI Command is valid.																																															
	                              																																																	
-313	hci_error_rej_by_remote_no_res				Indicates that an incoming connection was rejected due to limited      
													resources. 																																															
	                              																																																	
-314	hci_error_rej_by_remote_sec					Indicates that a connection was rejected due to security requirements  
													not being fulfilled, like authentication or pairing. 																																															
	                              																																																	
-315	hci_error_rej_by_remote_pers				Indicates that a connection was rejected because this device does not  
													accept the BD_ADDR. This may be because the device will only accept      
													connections from specific BD_ADDRs.																																															
	                              																																																	
-316	hci_error_host_timeout						Indicates that the Connection Accept Timeout has been exceeded for this
													connection attempt 																																															
	                              																																																	
-317	hci_error_unsupported_feature				Indicates that a feature or parameter value in an LMP message or HCI   
													Command is not supported.																																															
	                              																																																	
-318	hci_error_illegal_format					Indicates that at least one of the HCI command parameters is invalid.
	                              																																																	
-319	hci_error_oetc_user							Indicates that the user on the remote device terminated the 																																															           
													connection.																																															
	                              																																																	
-320	hci_error_oetc_low_resource					Indicates that the remote device terminated the connection because of  
													low resources. 																																															
	                              																																																	
-321	hci_error_oetc_powering_off					Indicates that the remote device terminated the connection because the 
													device is about to power off.																																															
	                              																																																	
-322	hci_error_conn_term_local_host				Indicates that the local device terminated the connection.																																															
	                              																																																	
-323	hci_error_auth_repeated						Indicates that the controller is disallowing an authentication or      
													pairing procedure because too little time has elapsed since the last     
													authentication or pairing attempt failed. 																																															
	                              																																																	
-324	hci_error_pairing_not_allowed				Indicates that the device does not allow pairing. For example,
													device only allows pairing during a certain time window after some user  
													input allows pairing.																																															
	                              																																																	
-325	hci_error_unknown_lmp_pdu					Indicates that the controller has received an unknown LMP opcode. 
	                              																																																	
-326	hci_error_unsupported_rem_feat				Indicates that the remote device does not support the feature          
													associated with the issued command or LMP PDU.																																															
	                              																																																	
-327	hci_error_sco_offset_rejected				Indicates that the offset requested in the LMP_SCO_link_req message has
													been rejected.																																															
	                              																																																	
-328	hci_error_sco_interval_rejected				Indicates that the interval requested in the LMP_SCO_link_req message  
													has been rejected. 																																															
	                              																																																	
-329	hci_error_sco_air_mode_rejected				Indicates that the air mode requested in the LMP_SCO_link_req message  
													has been rejected. 																																															
	                              																																																	
-330	hci_error_invalid_lmp_parameters			Indicates that some LMP message parameters were invalid. This shall be 
													used when :                                                 																																															             
													- The PDU length is invalid.                                																																															             
													- A parameter value must be even.                           																																															             
													- A parameter is outside of the specified range.            																																															             
													- Two or more parameters have inconsistent values. 																																															
																																																
-331	hci_error_unspecified						Indicates that no other error code specified is appropriate to use. 
																																																
-332	hci_error_unsupp_lmp_param					Indicates that an LMP message contains at least one parameter value    
													that is not supported by the controller at this time. This is normally   
													used after a long negotiation procedure,																																															
													LMP_hold_req, LMP_sniff_req and LMP_encryption_key_size_req message      
													exchanges.																																															
																																																
-333	hci_error_role_change_not_allowed			Indicates that a controller will not allow a role change at this       
													time. 																																															
	                                  																																																			
-334	hci_error_lmp_response_timeout				Indicates that an LMP transaction failed to respond within the LMP     
													response timeout. 																																															
	                                  																																																			
-335	hci_error_lmp_transaction_collision			Indicates that an LMP transaction has collided with the same           
													transaction that is already in progress.																																															
	                                    																																																		
-336	hci_error_lmp_pdu_not_allowed				Indicates that a controller sent an LMP message with an opcode that was
													not allowed.																																															
	                                  																																																			
-337	hci_error_enc_mode_not_acceptable			Indicates that the requested encryption mode is not acceptable at this 
													time. 																																															
	                                  																																																			
-338	hci_error_unit_key_used						Indicates that a link key can not be changed because a fixed unit key  
													is being used.																																															
	                                  																																																			
-339	hci_error_qos_not_supported					Indicates that the requested Quality of Service is not supported.
	                                  																																																			
-340	hci_error_instant_passed					Indicates that an LMP PDU that includes an instant can not be performed
													because the instant when this would have occurred has passed.
	                                  																																																			
-341	hci_error_pair_unit_key_no_support			Indicates that it was not possible to pair as a unit key was requested 
													and it is not supported.																																															
	                                    																																																		
-342	hci_error_different_transaction_collision	Indicates that an LMP transaction was started that collides with an    
													ongoing transaction.																																															
																																																
-343	hci_error_scm_insufficient_resources		Insufficient resources.																																															
																																																
-344	hci_error_qos_unacceptable_parameter		Indicates that the specified quality of service parameters could not be
													accepted at this time, but other parameters may be acceptable.
																																																
-345	hci_error_qos_rejected						Indicates that the specified quality of service parameters can not be  
													accepted and QoS negotiation should be terminated.																																															
																																																
-346	hci_error_channel_class_no_support			Indicates that the controller can not perform channel classification   
													because it is not supported.																																															
																																																
-347	hci_error_insufficient_security				Indicates that the HCI command or LMP message sent is only possible on 
													an encrypted link.																																															
																																																
-348	hci_error_param_out_of_mand_range			Indicates that a parameter value requested is outside the mandatory    
													range of parameters for the given HCI command or LMP message.
																																																
-349	hci_error_scm_no_longer_reqd				longer required.																																															
																																																
-350	hci_error_role_switch_pending				Indicates that a Role Switch is pending. This can be used when an HCI  
													command or LMP message can not be accepted because of a pending role     
													switch. This can also be used to notify a peer device about a pending    
													role switch.																																															
																																																
-351	hci_error_scm_param_change_pending			Parameter change pending. 																																															
																																																
-352	hci_error_resvd_slot_violation				Indicates that the current Synchronous negotiation was terminated with 
													the negotiation state set to Reserved Slot Violation.
																																																
-353	hci_error_role_switch_failed				Indicates that a role switch was attempted but it failed and the       
													original piconet structure is restored. The switch may have failed       
													because the TDD switch or piconet switch failed.																																															
																																																
-354	hci_error_unrecognised						Unrecognised error.
--------
-397	oti_LQ_BT_address_not_in_ps					There where no Bluetooth Address for the Link Quality (LQ) Tester at expected location in Persistent Store (PS)
-398	oti_error_slc_other_bt_device				During the Link Quality test a connection was made to some other BT device than the Link Quality Tester
-399	oti_connection_to_lq_failed					It was not possible to set up a connection to the Link Quality Tester
*/
WLPINTERFACE_API int WLPCsrLinkQualityTest(int Handle);

//-----------------------------------------------------------------------------------------
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param iMeasuredFreq	The measured frequency in Hz
// \param iRefFreq		The reference frequency in Hz, 26000000
// 
// \return				WLP_SUCCESS on succesful execution, error codes described below if not successful
//
// Function
// Inclusive Time		A successfull execution takes approx. 5 second
//-----------------------------------------------------------------------------------------
//--------------------- Possible error codes returned -------------------------------------
/*
-301	fail_measured_freq_out_of_range				The measured frequency should be within +/- VAL_CRYSTAL_TRIM_ALLOWED_OFFSET      
													Otherwise a problem exist in e.g. an erronous crystal or capacitors
	                              																																																	
-302	fail_freq_crystal_trim_val_too_low			The calculated new trim value to be stored in the Bluetooth Controller must be
													within VAL_CRYSTAL_TRIM_MIN and VAL_CRYSTAL_TRIM_MAX.
													This error code indicates that the newly calculated trim value is below VAL_CRYSTAL_TRIM_MIN
	                              																																																	
-303	fail_freq_crystal_trim_val_too_high			The calculated new trim value to be stored in the Bluetooth Controller must be
													within VAL_CRYSTAL_TRIM_MIN and VAL_CRYSTAL_TRIM_MAX.
													This error code indicates that the newly calculated trim value is above VAL_CRYSTAL_TRIM_MAX
*/
WLPINTERFACE_API int WLPCsrCrystalTrim(int Handle, int iMeasuredFreq, int iRefFreq);

//-----------------------------------------------------------------------------------------
// Perform Crystal Trimming via Selected Transport Interface
//-----------------------------------------------------------------------------------------
//
// \param Handle				Handle to attached device obtained from a previous connect command
// \param CsrPsTransportConfig	Parameter to denote the Transport to the CSR BlueCore to use, defined by ECsrPsTransportConfig
//								Examples of setups to select transport could be:
//								PC <-USB-> ARM Controller <-UART-> CSR BlueCore	-  To use this path you would select UART Transport to the BlueCore
//								PC <-SPI-> CSR BlueCore							-  To use this path you would select SPI Transport to the BlueCore
//
// \param iMeasuredFreq			The measured frequency in Hz
// \param iRefFreq				The reference frequency in Hz, 26000000
// 
// \return						WLP_SUCCESS on succesful execution, error codes described below if not successful
//
// Function
// Inclusive Time				A successfull execution takes approx. 5 second
//-----------------------------------------------------------------------------------------
//--------------------- Possible error codes returned -------------------------------------
/*
-301	fail_measured_freq_out_of_range				The measured frequency should be within +/- VAL_CRYSTAL_TRIM_ALLOWED_OFFSET      
													Otherwise a problem exist in e.g. an erronous crystal or capacitors
	                              																																																	
-302	fail_freq_crystal_trim_val_too_low			The calculated new trim value to be stored in the Bluetooth Controller must be
													within VAL_CRYSTAL_TRIM_MIN and VAL_CRYSTAL_TRIM_MAX.
													This error code indicates that the newly calculated trim value is below VAL_CRYSTAL_TRIM_MIN
	                              																																																	
-303	fail_freq_crystal_trim_val_too_high			The calculated new trim value to be stored in the Bluetooth Controller must be
													within VAL_CRYSTAL_TRIM_MIN and VAL_CRYSTAL_TRIM_MAX.
													This error code indicates that the newly calculated trim value is above VAL_CRYSTAL_TRIM_MAX
*/
WLPINTERFACE_API int WLPCsrCrystalTrimSelectTrans(int Handle, int CsrPsTransportConfig, int iMeasuredFreq, int iRefFreq);


//-----------------------------------------------------------------------------------------
// Read analog value from the AD converters on the microcontroller
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Id			The Analog input to read from
// \param pValue		Pointer to returned analog value (pointer to single element). 
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPAnalogInputRead(int Handle, int Id, int* pValue);

//-----------------------------------------------------------------------------------------
// Write a setting into the attached device. The setting must be identified using the key
// string
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param pKey			pointer to string that identifies the setting to access.
//						Example: "MCU.Production.SerialNumber"
// \param pValue		Pointer to array of values to write
// \param Length		Length of array to write (number of items in array)
// 
// \return				Connection return code. 
//                      0 if command failed
//                      1 if command succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPSettingWrite(int Handle, const char* pKey, const int* pValue, int Length);

//-----------------------------------------------------------------------------------------
// Read a setting from the attached device. The setting must be identified using the key
// string
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param pKey			pointer to string that identifies the setting to access.
//						Example: "MCU.Production.SerialNumber"
// \param Values		Pointer to array where the read values must be copied to
//                      The array buffer must be provided by the caller
// \param pLength		Pointer to returned settings length (number of items in the Values array)
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPSettingRead(int Handle, const char* pKey, int Values[1024], int* pLength);

//-----------------------------------------------------------------------------------------
// Write a settings file (Nebula .xml file) into the attached device
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param pFilename		File path to settings file to program into device.
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPSettingWriteFile(int Handle, const char* pFilename);

//-----------------------------------------------------------------------------------------
// Read all settings from the attached device and save into a nebula .xml file
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param pFilename		File path to settings file to write to disk
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPSettingReadFile(int Handle, const char* pFilename);

//-----------------------------------------------------------------------------------------
// Finalize the device so it becomes ready for end-users.
// This function changes the boot flag, so the device starts up in the streamer application image.
//
// \param Handle		Handle to attached device obtained from a previous connect command
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPFinalize(int Handle);

//-----------------------------------------------------------------------------------------
// Reset the device.
//
// \param Handle		Handle to attached device obtained from a previous connect command
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPReset(int Handle);

#if 0
//-----------------------------------------------------------------------------------------
// Send a Nearlink remote command.
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Source		Nearlink source address (1 byte vendor id, 4 bytes address)
// \param Source		Nearlink destination address (1 byte vendor id, 4 bytes address)
// \param Message		Nearlink message (variable length, but max. 64 bytes)
// \param Length		The actual message length in bytes
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPNearlinkRemote(int Handle, unsigned char Source[5], unsigned char Destination[5], unsigned char Message[64], int Length);
#endif

//-----------------------------------------------------------------------------------------
// Set the Nearlink direction.
//
// \param Handle		Handle to attached device obtained from a previous connect command
// \param Direction		0 for Tx - the WLP drives the NL signal
//                      1 for Rx – the WLP does not drive the NL signal
// 
// \return				Connection return code. 
//                      0 if communication connection failed
//                      1 if communication succeded
//-----------------------------------------------------------------------------------------
WLPINTERFACE_API int WLPNearlinkDirection(int Handle, int Direction);

#ifdef SYSTEM_TEST
}
#endif
