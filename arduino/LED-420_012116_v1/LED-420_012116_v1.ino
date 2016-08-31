#include <LPD8806.h>
/*
* ########################
* VARIABLES
* ########################
*/
//this sketch is recieving a 1 or 0 based on a hall effect sensor, that tells the led strip to either trail foward or reverse
// LED STRIP SETUP
int nLEDs = 480;
int lengthBasedWait = 0;
// Chose 2 pins for output; can be any valid output pins:
int dataPin  = 11;
int clockPin = 13;
// First parameter is the number of LEDs in the strip - strips are 32 LEDs per meter
LPD8806 strip = LPD8806(nLEDs);
//LPD8806 strip = LPD8806(nLEDs, dataPin, clockPin);
// NUMBER OF LEDs
int minPixel = 1;
int maxPixel = nLEDs;
// POSITION AND DIRECTION VARIABLES
int atPixel = 1;
int dir = 1;

// Serial Read
int incomingByte = -1;
/*
* ########################
* FUNCTIONS
* ########################
*/
// SETUP*********************
void setup() {
 pinMode(dataPin, OUTPUT);
 pinMode(clockPin, OUTPUT);
 //pinMode(ledPin, OUTPUT);
 
 Serial.begin(57600);
 strip.begin();
 //turnOffLights();

}
void turnOffLights(){
  
  while(int a = maxPixel >0){
      dir=-1;
      strip.setPixelColor(atPixel, strip.Color(0, 0, 0)); // Set new pixel 'on'
      strip.show();
      a = a + dir;
  }
}
   
//LOOP**********
void loop() {
   if (Serial.available() > 0) {
     incomingByte = Serial.read();
   }
   
   if(incomingByte ==1){
    
    if(atPixel<maxPixel){
      dir = 1;
      strip.setPixelColor(atPixel, strip.Color(133, 205, 198)); // Set new pixel 'on'
      strip.show();
      atPixel = atPixel + dir;

    }
    else if(atPixel==maxPixel){
      incomingByte = -1;
      return;
    }

   }
   else if(incomingByte ==0)
   {

    if(atPixel>0){
      dir=-1;
      strip.setPixelColor(atPixel, strip.Color(0, 0, 0)); // Set new pixel 'on'
      strip.show();
      atPixel = atPixel + dir;
    }
    else if (atPixel ==0){
      strip.setPixelColor(atPixel, strip.Color(0, 0, 0)); // Set new pixel 'on'
      strip.show();
      incomingByte =-1;
      return;
      
    }    
   }
  }


