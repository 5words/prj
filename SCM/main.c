/*----------------------------------------------------------------------------
 * CMSIS-RTOS 'main' function template
 *---------------------------------------------------------------------------*/
 
#include "RTE_Components.h"
#include  CMSIS_device_header
#include "cmsis_os2.h"
#include <string.h>
#include "LM75A.h"

 
#ifdef RTE_Compiler_EventRecorder
#include "EventRecorder.h"
#endif
 
 void LM75A_init(_Bool Mode);
 short readTemp(int ch);
 short Tbuffer [4][8];
 int f1,f2;
 
/*----------------------------------------------------------------------------
 * Application main thread
 *---------------------------------------------------------------------------*/
void app_main (void *argument) {  // ...  read temperature information from LM75
	int whichChannel=0;
	int t;
	LM75A_init(0);
	while(1){
		
		readTemp(whichChannel);
		whichChannel++;
		whichChannel&=0x03;
//		if(whichChannel==3)whichChannel=0;
		osDelay(250);
		f1++;
	}
	
 
  
   
}
 
int main (void) {
 
  // System Initialization
  SystemCoreClockUpdate();
#ifdef RTE_Compiler_EventRecorder
  // Initialize and start Event Recorder
  EventRecorderInitialize(EventRecordError, 1U);
#endif
  // ...
	memset(Tbuffer,0,sizeof(Tbuffer));
  osKernelInitialize();                 // Initialize CMSIS-RTOS
  osThreadNew(app_main, NULL, NULL);    // Create application main thread
  osKernelStart();                      // Start thread execution
	LM75A_init(0);
	
	for (;;) {}
}



int I2C_receive(data)
{

	return 0;
	
}

short readTemp(int ch)
{
	short Temp,m,res;
	for(int n=7;n>0;n--)
	{
		Tbuffer[ch][n]=Tbuffer[ch][n-1];		
	}
	Tbuffer[ch][0]=Temp;
	for(int i=0;i<=7;i++)
	{
		res+=Tbuffer[ch][i];
	}
	res/=8;
	return res;
}
