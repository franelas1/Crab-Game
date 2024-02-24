/*
  Analog input, analog output, serial output

  Reads an analog input pin, maps the result to a range from 0 to 255 and uses
  the result to set the pulse width modulation (PWM) of an output pin.
  Also prints the results to the Serial Monitor.

  The circuit:
  - potentiometer connected to analog pin 0.
    Center pin of the potentiometer goes to the analog pin.
    side pins of the potentiometer go to +5V and ground
  - LED connected from digital pin 9 to ground through 220 ohm resistor

  created 29 Dec. 2008
  modified 9 Apr 2012
  by Tom Igoe

  This example code is in the public domain.

  https://www.arduino.cc/en/Tutorial/BuiltInExamples/AnalogInOutSerial
*/

// These constants won't change. They're used to give names to the pins used:
const int analogInPinY = A0;  // Analog input pin that the potentiometer is attached to
const int analogInPinX = A1;
//const int analogOutPinX = 12;  // Analog output pin that the LED is attached to
//const int analogOutPinY = 11;

int sensorValueX = 0;  // value read from the pot
int outputValueX = 0;  // value output to the PWM (analog out)
int sensorValueY = 0;  
int outputValueY = 0;  

//variables for value smoothing
int smoothedVal  = 0;    // smoothed result
int samples      = 4;    // amount of samples

void setup() {
  // initialize serial communications at 9600 bps:
  Serial.begin(9600);
}

void loop() {
  // read the analog in value:
  sensorValueX = analogRead(analogInPinX);
  //sensorValueY = analogRead(analogInPinY);

  //smoothing analog input values
  smoothedVal = smoothedVal + ((sensorValueX - smoothedVal)/samples);

  // map it to the range of the analog out:
  outputValueX = map(smoothedVal, 0, 1023, 95, 705);
  //outputValueY = map(sensorValueY, 0, 1023, 0, 255);

  // change the analog out value:
  //analogWrite(analogOutPinX, outputValueX);
  //analogWrite(analogOutPinY, outputValueY);
  
  // threshhold for max & min NOT WORKING
  //if (sensorValueX > 900) outputValueX = 255;
  //if (sensorValueY > 900) outputValueY = 255;
  //if (sensorValueX < 100) outputValueX = 0;
  //if (sensorValueY < 100) outputValueY = 0;
  if (outputValueX > 695) outputValueX = 705;
  if (outputValueX < 105) outputValueX = 95;

  // print the results to the Serial Monitor:
  // Serial.print("sensorX = ");
  //Serial.print(sensorValueX);
  //Serial.print("\t");
  // Serial.print("\t outputX = ");
  //Serial.print(outputValueX);
  //Serial.print("\t");
  // Serial.print("\t sensorY = ");
  //Serial.print(sensorValueY);
  //Serial.print("\t");
  // Serial.print("\t outputY = ");
  Serial.println(outputValueX);

  // wait 2 milliseconds before the next loop for the analog-to-digital
  // converter to settle after the last reading:
  delay(10);
}