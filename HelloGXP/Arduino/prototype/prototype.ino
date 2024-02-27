// These constants won't change. They're used to give names to the pins used:
const int analogInPinY = A0;      // Analog input pin that the potentiometer is attached to
const int analogInPinX = A1;
const int button1 = 6;
const int button2 = 5;
const int timer = 20;

// variables for IO
int sensorValueX = 0;     // value read from the pot
int outputValueX = 0;   // value output to the PWM (analog out)
int button1state = 0;  // button state value
int button2state = 0;

//variables for value smoothing
int smoothedVal  = 0;    // smoothed result
int samples      = 1;      // amount of samples

void setup() {
  // initialize button as input
  pinMode(button1, INPUT);
  pinMode(button2, INPUT);
  // initialize serial communications at 9600 bps:
  Serial.begin(9600);
}

void loop() {
  // read the analog in value
  sensorValueX = analogRead(analogInPinX);

  // read digital in value 
  button1state = digitalRead(button1);
  button2state = digitalRead(button2);

  //smoothing analog input values
  smoothedVal = smoothedVal + ((sensorValueX - smoothedVal)/samples);

  // map it to the range of the analog out:
  outputValueX = map(smoothedVal, 0, 1023, -100, 100);
  
  // threshhold for max & min 
  if(smoothedVal > 512 - 50 && smoothedVal < 512 + 50) outputValueX = 0;
  if(smoothedVal > 1023 - 100) outputValueX = 100;
  if(smoothedVal < 0 + 100) outputValueX = -100;

  if (millis() % timer == 0){
  Serial.print(outputValueX);
  Serial.print(" ");
  Serial.print(button1state);
  Serial.print(" ");
  Serial.println(button2state);}

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
