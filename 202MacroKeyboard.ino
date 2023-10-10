#include <ResKeypad.h>
#include <EEPROM.h>
#include "CH9329_Keyboard.h"

// ボーレート設定値計算マクロ。最後の"+0.5"は、四捨五入させるため？？
#define USART0_BAUD_RATE(BAUD_RATE) ((float)(F_CPU * 64 / (16 * (float)BAUD_RATE)) + 0.5)
#define USART0_AVAILABLE (USART0.STATUS & USART_RXCIF_bm)

#define CH9329_PACKET_HEAD1 0
#define CH9329_PACKET_HEAD2 1
#define CH9329_PACKET_ADDR  2
#define CH9329_PACKET_CMD   3
#define CH9329_PACKET_LEN   4
#define CH9329_PACKET_DATA  5
#define CH9329_CMD_READ_MY_HID_DATA  0x87

/* カスタムHIDデータのデータ形式　
struct myHidData{
  uint8_t num;          //キー番号
  uint8_t keys[6];      //キー 先頭が0だったらカスタムボタン(PC側で動作を設定)。それ以外は、キーボードボタン。
  uint8_t modifiers;    //修飾キー
}
*/
const uint8_t MY_HID_DATA_LEN = 8;      //1 + 1 + 6 = 8
const uint8_t KEY_CONFIGDATA_LEN = 7;   //カスタムHIDデータからキー番号の領域を無くしたもの。EEPROMに格納されるデータ。

const uint8_t SW_PIN = PIN_PA1;
const uint8_t LED1_PIN = PIN_PA2;
const uint8_t LED2_PIN = PIN_PA3;
const uint8_t RECEIVE_PACKET_LENGTH = 14;
volatile uint8_t reportData[KEY_REPORT_DATA_LENGTH] = {};
volatile uint8_t receivePacketPosition = 0;
volatile uint8_t receivePacket[RECEIVE_PACKET_LENGTH] = {};
const int KeyNum = 7;
const int threshold[KeyNum] = {
  // 次の数列は、しなぷすのハード製作記の回路設計サービスで計算して得られたもの
  42, 165, 347, 511, 641, 807, 965
};
ResKeypad keypad(SW_PIN, KeyNum, threshold, NULL, false);

// USART初期化
void USART0_init(void) {
  //ピン初期化
  PORTA.DIR &= ~PIN7_bm;  //入力
  PORTA.DIR |= PIN6_bm;   //出力
  //ボーレート設定
  USART0.BAUD = (uint16_t)USART0_BAUD_RATE(CH9329_DEFAULT_BAUDRATE);
  //受信・送信許可
  USART0.CTRLB |= USART_TXEN_bm;
  USART0.CTRLB |= USART_RXEN_bm;
  //受信完了割り込み許可
  USART0.CTRLA |= USART_RXCIE_bm;
}

// 一つの値送信
void USART0_sendValue(uint8_t c) {
  while (!(USART0.STATUS & USART_DREIF_bm)) {
    ;
  }
  USART0.TXDATAL = c;
}

//一つの値受信
uint8_t USART0_read() {
  while (!USART0_AVAILABLE) {
    ;
  }
  return USART0.RXDATAL;
}

// 受信割り込み
ISR(USART0_RXC_vect) {
  uint8_t tempData = USART0.RXDATAL;
  if (receivePacketPosition == RECEIVE_PACKET_LENGTH) {
    return;
  }
  
  receivePacket[receivePacketPosition] = tempData;
  if (receivePacketPosition == CH9329_PACKET_LEN) {
    // 受信データがLENの場合で、値が8より多い場合はエラー停止。
    if (tempData > MY_HID_DATA_LEN) {
      digitalWriteFast(LED2_PIN, HIGH);
      while(true);
    }
    receivePacketPosition += MY_HID_DATA_LEN - tempData;
  }
  receivePacketPosition++;
}

// 複数の値送信
void USART0_sendValue(uint8_t* c, size_t length) {
  for (size_t i = 0; i < length; i++ ) {
    USART0_sendValue(c[i]);
  }
}

void setup() {
  USART0_init();
  CH9329_Keyboard.begin();

  pinModeFast(SW_PIN, INPUT);
  pinModeFast(LED2_PIN, OUTPUT);
  delay(5000);
}

void loop() {
  // 受信バッファインデックスがバッファの最後(+1)を指している場合、受信完了とみなして処理を行う。
  if (receivePacketPosition == RECEIVE_PACKET_LENGTH) {
    receivePacketPosition = 0;
    // HIDカスタムデータの場合、解析し、EEPROMに保存。
    if (receivePacket[CH9329_PACKET_CMD] == CH9329_CMD_READ_MY_HID_DATA){
      uint8_t* hidData = receivePacket + CH9329_PACKET_DATA;
      uint8_t eepromAddress = hidData[0] * KEY_CONFIGDATA_LEN;
      for(int i = 1; i < MY_HID_DATA_LEN; i++){
        EEPROM.update(eepromAddress, hidData[i]);
        eepromAddress++;
      }
    }
  }

  int key = keypad.GetKey();
}
