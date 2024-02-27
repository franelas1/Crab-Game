// These constants won't change. They're used to give names to the pins used:
const int analogInPinY = A0;      // Analog input pin that the potentiometer is attached to
const int analogInPinX = A1;

int sensorValueX = 0;     // value read from the pot
int outputValueX = 0;   // value output to the PWM (analog out)

//variables for value smoothing
int smoothedVal  = 0;    // smoothed result
int samples      = 5;      // amount of samples

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
  outputValueX = map(smoothedVal, 0, 1023, -100, 100);
  
  // threshhold for max & min 
  if(smoothedVal > 512 - 50 && smoothedVal < 512 + 50) outputValueX = 0;
  if(smoothedVal > 1023 - 100) outputValueX = 100;
  if(smoothedVal < 0 + 100) outputValueX = -100;

  Serial.println(outputValueX);

  delay(10);
}
