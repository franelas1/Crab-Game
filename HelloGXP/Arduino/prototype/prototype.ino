// These constants won't change. They're used to give names to the pins used:
const int analogInPin1 = A2;      // Analog input pin that the potentiometer is attached to
const int analogInPin2 = A3;
const int button11 = 18;
const int button12 = 19;
const int button21 = 9;
const int button22 = 21;

const int timer = 16;

// variables for IO
int sensorValue1 = 0;     // value read from the pot
int outputValue1 = 0;     // value output to the PWM (analog out)
int sensorValue2 = 0;     // value read from the pot
int outputValue2 = 0;     // value output to the PWM (analog out)
int button11state = 0;    // button state value
int button12state = 0;    // button state value
int button21state = 0;    // button state value
int button22state = 0;    // button state value

//variables for value smoothing
int smoothedVal  = 0;    // smoothed result
int samples      = 1;      // amount of samples

void setup() {
  // initialize button as input
  pinMode(button11, INPUT_PULLUP);
  pinMode(button12, INPUT_PULLUP);
  pinMode(button21, INPUT_PULLUP);
  pinMode(button22, INPUT_PULLUP);
  // initialize serial communications at 9600 bps:
  Serial.begin(9600);
}

void loop() {
  // read the analog in value
  sensorValue1 = analogRead(analogInPin1);
  sensorValue2 = analogRead(analogInPin2);

  // read digital in value 
  button11state = !digitalRead(button11);
  button12state = !digitalRead(button12);
  button21state = !digitalRead(button21);
  button22state = !digitalRead(button22);

  //smoothing analog input values
  //smoothedVal = smoothedVal + ((sensorValueX - smoothedVal)/samples);

  // map it to the range of the analog out:
  outputValue1 = map(sensorValue1, 0, 1023, -100, 100);
  outputValue2 = map(sensorValue2, 0, 1023, -100, 100);
  
  // threshhold for max & min 
  if(sensorValue1 > 512 - 100 && sensorValue1 < 512 + 100) outputValue1 = 0;
  if(sensorValue1 > 1023 - 100) outputValue1 = 100;
  if(sensorValue1 < 0 + 100) outputValue1 = -100;

  if(sensorValue2 > 512 - 100 && sensorValue2 < 512 + 100) outputValue2 = 0;
  if(sensorValue2 > 1023 - 100) outputValue2 = 100;
  if(sensorValue2 < 0 + 100) outputValue2 = -100;

  if (millis() % timer == 0){
  Serial.print(outputValue1);
  Serial.print(" ");
  Serial.print(button11state);
  Serial.print(" ");
  Serial.print(button12state);
  Serial.print(" ");
  Serial.print(outputValue2);
  Serial.print(" ");
  Serial.print(button21state);
  Serial.print(" ");
  Serial.println(button22state);}

  //delay(20);
}

void padPrint( int value, int width)
{
// pads values with leading zeros to make the given width
char valueStr[6]; // large enough to hold an int
   itoa (value, valueStr, 10);
   int len = strlen(valueStr);
   if(len < width){
     len = width-len;
     while(len--)
      Serial.print('0');
   }
  Serial.println(valueStr);   
}
