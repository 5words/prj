#include <LPC11XX.h>
#include "LM75A.h"

#define I2CONSET_I2EN		0x00000040  // I2C Control Set Register
#define I2CONSET_AA			0x00000004
#define I2CONSET_SI			0x00000008
#define I2CONSET_STO		0x00000010
#define I2CONSET_STA		0x00000020

#define I2CONCLR_AAC		0x00000004  // I2C Control clear Register
#define I2CONCLR_SIC		0x00000008
#define I2CONCLR_STAC		0x00000020
#define I2CONCLR_I2ENC		0x00000040

#define I2SCLH_SCLH			0x0000003c  // I2C SCL Duty Cycle High Reg
#define I2SCLL_SCLL			0x0000003c  // I2C SCL Duty Cycle Low Reg


void LM75A_init(_Bool Mode)
{
	LPC_SYSCON->SYSAHBCLKCTRL|= (0x01<<5);
	LPC_SYSCON->PRESETCTRL|=(0x01<<1);
	LPC_IOCON->PIO0_4&=(~0x3f);
	LPC_IOCON->PIO0_5&=(~0x3f);
	LPC_IOCON->PIO0_4|=(0x01);
	LPC_IOCON->PIO0_5|=(0x01);
	if (Mode == 1){           //Fast mode
		LPC_IOCON->PIO0_4|=(0x10<<8);
		LPC_IOCON->PIO0_5|=(0x10<<8);		
	}else{                   //Standard mode
		LPC_IOCON->PIO0_4|=(0x01<<8);
		LPC_IOCON->PIO0_5|=(0x01<<8);	
	}
	LPC_I2C->CONCLR|=(0x3f);
	LPC_I2C->CONSET|=(0x01<<6);
}


void LM75Astart()
{
	LPC_I2C->CONSET= I2CONSET_STA;
}

void I2C_IRQHandler(void)
{
	char stat;
	stat = LPC_I2C->STAT;
	
	switch (Stat) 
	{
		case 0x08:			// A Start condition is issued.
		case 0x10:			// A repeated started is issued
			LPC_I2C->DAT = RD_AD7416_TEMP;
			LPC_I2C->CONCLR = (I2CONCLR_SIC | I2CONCLR_STAC);
			break;

		case 0x40:	// Master Receive, SLA_R has been sent
			LPC_I2C->CONSET = I2CONSET_AA;	// assert ACK after data is received
			LPC_I2C->CONCLR = I2CONCLR_SIC;
			break;
	
		case 0x50:	// Data byte has been received, regardless following ACK or NACK
			ad7416Temp = LPC_I2C->DAT<<8;
			LPC_I2C->CONCLR = I2CONCLR_SIC | I2CONCLR_AAC;
			break;

		case 0x58:
			ad7416Temp |= LPC_I2C->DAT;
			LPC_I2C->CONSET = I2CONSET_STO;
			LPC_I2C->CONCLR = I2CONCLR_SIC;
			tempBuf[saveTempPtr++] = ad7416Temp;
			if (saveTempPtr>=NUM_TEMP_BUF) saveTempPtr =0;
			break;
	
		case 0x48:
			LPC_I2C->CONSET = I2CONSET_STA;	// Set Repeated-start flag
			LPC_I2C->CONCLR = I2CONCLR_SIC;
			break;

		case 0x38:			// Arbitration lost, in this example, we don't deal with multiple master situation
		default:
			LPC_I2C->CONSET = I2CONSET_STO;
			LPC_I2C->CONCLR = I2CONCLR_SIC;	
			break;
	}
}