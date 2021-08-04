#include <Servo.h>

Servo s1, s2, sgarra, sp;//declara nome dos servos;
float degtheta1;
float degtheta2;
float dt1 = 0, dt2 = 0, dy = 0, Zanterior = 0;

void printSerial(String zz, float xx, float yy, float dg1, float dg2){
    // Sending the parts to Serial Monitor
      Serial.print("theta1");
      Serial.print(": ");
      Serial.print(dg1);
      Serial.print(" - ");
      Serial.print("theta2");
      Serial.print(": ");
      Serial.print(dg2);
      Serial.print(" - z: ");
      Serial.println(zz);
      Serial.print("x: ");
      Serial.print(xx);
      Serial.print(" - y: ");
      Serial.println(yy);
}

void moveZ(float z){
  z *= (-1);
  float  deltaZ = z - Zanterior;
  
  if(deltaZ <= 0){
    for(float i = 0; i >= deltaZ; i--){
      float altura = ((Zanterior + i)*3.6);
      sp.write(altura);
      delay(30);
    }
  }
  else{
    for(float i = 0; i <= deltaZ; i++){
      float altura = ((Zanterior + i)*3.6);
      sp.write(altura);
      delay(30);
    }
  }
  Zanterior = z;
}
void moveServos(float newdt1, float newdt2, float z){

  moveZ(z);
  
  float deltaOne = newdt1 - dt1;
  float deltaTwo = newdt2 - dt2;

            if (deltaOne >= 0 && deltaTwo >= 0)
            {
                if(deltaOne > deltaTwo)
                {
                    for(int i = 0; i <= deltaOne; i++)
                    {
                        dt1 ++;
                        s1.write(dt1);
                        delay(15);
                        if(i <= deltaTwo)
                        {
                            dt2 ++;
                             s2.write(dt2 + 90);
                            delay(15);
                        }                        
                    }
                }
                else if(deltaOne < deltaTwo)
                {
                    for (int i = 0; i <= deltaTwo; i++)
                    {
                        dt2 ++;
                         s2.write(dt2 + 90);
                        delay(15);
                        if (i <= deltaOne)
                        {
                            dt1 ++;
                            s1.write(dt1);
                            delay(15);
                        }                        
                    }
                }
                else if(deltaOne == deltaTwo)
                {

                    for(int i = 0; i <= deltaOne; i++)
                    {
                        dt1 ++;
                        dt2 ++;
                        s1.write(dt1);
                        delay(15);
                         s2.write(dt2 + 90);
                        delay(15);
                    }
                }
            }
            else if (deltaOne >= 0 && deltaTwo < 0)
            {
                if (deltaOne >  abs(deltaTwo))
                {
                    for (int i = 0; i <= deltaOne; i++)
                    {
                        dt1++;
                        s1.write(dt1);
                        delay(15);
                        if (i <=  abs(deltaTwo))
                        {
                            dt2--;
                             s2.write(dt2 + 90);
                            delay(15);
                        }                        
                    }
                }
                else if (deltaOne <  abs(deltaTwo))
                {
                    for (int i = 0; i <=  abs(deltaTwo); i++)
                    {
                        dt2--;
                         s2.write(dt2 + 90);
                        delay(15);
                        if (i <= deltaOne)
                        {
                            dt1++;
                            s1.write(dt1);
                            delay(15);
                        }
                    }
                }
                else if (deltaOne ==  abs(deltaTwo))
                {
                    for (int i = 0; i <= deltaOne; i++)
                    {
                        dt1++;
                        dt2--;
                        s1.write(dt1);
                        delay(15);
                         s2.write(dt2 + 90);
                        delay(15);                                                                                         
                    }
                }
            }
            else if (deltaOne <= 0 && deltaTwo >= 0)
            {
                if ( abs(deltaOne) > deltaTwo)
                {
                    for (int i = 0; i <=  abs(deltaOne); i++)
                    {
                        dt1--;
                        s1.write(dt1);
                        delay(15);
                        if (i <= deltaTwo)
                        {
                            dt2++;
                             s2.write(dt2 + 90);
                            delay(15);   
                        }
                    }
                }
                else if ( abs(deltaOne) < deltaTwo)
                {
                    for (int i = 0; i <= deltaTwo; i++)
                    {
                        dt2++;
                         s2.write(dt2 + 90);
                        delay(15);   
                        if (i <=  abs(deltaOne))
                        {
                            dt1--;
                            s1.write(dt1);
                            delay(15);
                        }
                    }
                }
                else if ( abs(deltaOne) == deltaTwo)
                {
                    for (int i = 0; i <= deltaTwo; i++)
                    {
                        dt1--;
                        dt2++;
                        s1.write(dt1);
                        delay(15);
                         s2.write(dt2 + 90);
                        delay(15);   
                    }
                }
            }
            else if (deltaOne <= 0 && deltaTwo < 0)
            {
                if ( abs(deltaOne) >  abs(deltaTwo))
                {
                    for (int i = 0; i <= abs(deltaOne); i++)
                    {
                        dt1--;
                        s1.write(dt1);
                        delay(15);
                        if (i <=  abs(deltaTwo))
                        {
                            dt2--;
                            s2.write(dt2 + 90);
                           delay(15);   
                        }
                    }
                }
                else if ( abs(deltaOne) <  abs(deltaTwo))
                {
                    for (int i = 0; i <= abs(deltaTwo); i++)
                    {
                        dt2--;
                        s2.write(dt2 + 90);
                        delay(15);   
                        if (i <=  abs(deltaOne))
                        {
                            dt1--;
                            s1.write(dt1);
                            delay(15);
                        }
                    }
                }
                else if ( abs(deltaOne) ==  abs(deltaTwo))
                {
                    for (int i = 0; i <=  abs(deltaOne); i++)
                    {
                        degtheta1--;
                        degtheta2--;
                        s1.write(dt1);
                        delay(15);
                        s2.write(dt2 + 90);
                        delay(15);   
                    }
                }
            }

      }

void setup() {
  Serial.begin(9600);    //configura comunicação serial com 9600 bps
  sp.attach(9);
  sgarra.attach(8);
  s1.attach(10);
  s2.attach(11);
  
  s1.write(0);
  delay(500);
  s2.write(0 + 93);
  delay(500);
  sgarra.write(0);
  delay(500);
  sp.write(0);
  delay(500);
}

void(* resetFunc)(void)=0;//declara função reseta @endereço 0

void loop() {
   if (Serial.available()) //se byte pronto para leitura
   {
    while(Serial.read() > 0)      //verifica qual caracter recebido
    {
      String myString = Serial.readString();
      int commaIndex = myString.indexOf('.');
      int secondCommaIndex = myString.indexOf('.', commaIndex+1);
      int thirdCommaIndex = myString.indexOf('.', secondCommaIndex+1);
      
      String theta1 = myString.substring(0, commaIndex);
      String theta2 = myString.substring(commaIndex+1, secondCommaIndex);
      String z = myString.substring(secondCommaIndex+1, thirdCommaIndex); //To the end of the string 
      String determinant = myString.substring(thirdCommaIndex+1);

      float degtheta1 = theta1.toFloat();
      float degtheta2 = theta2.toFloat();

      //char det = determinant.toCharArray(0,1);

      float x = 130*(cos(degtheta1*3.14159265/180)) + 130*(cos((degtheta1*3.14159265/180) + (degtheta2*3.14159265/180)));
      float y = 130*(sin(degtheta1*3.14159265/180)) + 130*(sin((degtheta1*3.14159265/180) + (degtheta2*3.14159265/180)));

            
      char det = determinant.charAt(0);

      switch(det){
        case 'a':
        printSerial(z, x, y, degtheta1, degtheta2);
        moveServos(degtheta1, degtheta2, z.toFloat());   
        break;

        case 'b':
        Serial.println("Abre garra");
        sgarra.write(z.toFloat());//angulo para abrir
        break;

        case 'c':
        Serial.print("Fecha garra com angulo de: ");
        Serial. println(z);
        sgarra.write(z.toFloat());//anglulo para fechar
        break;
        
        case 'd':
        moveZ(z.toFloat()); //passa o valor de z e Zanterior do programa c#
        Serial.print("Junta prismatica vai a coordenada z: ");
        Serial.println(z);
        break;       

        case 'r':
        for(int i = 180; i > 0; i--){
          s1.write(degtheta1-i); 
          s2.write(90); 
          sgarra.write(0); 
          sp.write(0); 
        }
        Serial.println("Reseta Arduino");
        resetFunc();//chama a função reseta
        break;
        
        default:
        Serial.println("O caso não foi reconhecido pelo Arduino");
        break;
      }


      }
  }
}
