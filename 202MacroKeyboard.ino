#include <ResKeypad.h>
#include "CH9329_Keyboard.h"

// ボーレート設定値計算マクロ。最後の"+0.5"は、四捨五入させるため？？
#define USART0_BAUD_RATE(BAUD_RATE) ((float)(F_CPU * 64 / (16 * (float)BAUD_RATE)) + 0.5)
#define USART0_AVAILABLE (USART0.STATUS & USART_RXCIF_bm)

#define CH9329_PACKET_HEAD1 0
#define CH9329_PACKET_HEAD2 1
#define CH9329_PACKET_ADDR  2
#define CH9329_PACKET_CMD   3
#define CH9329_PACKET_LEN   4
#define CH9329_CMD_READ_MY_HID_DATA  0x87

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
    if (tempData > 8) {
      digitalWriteFast(LED2_PIN, HIGH);
      while(true);
    }
    receivePacketPosition += 8 - tempData;
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
  if (receivePacketPosition == CH9329_PACKET_LEN) {
    receivePacketPosition = 0;
    if (receivePacket[CH9329_PACKET_CMD] == CH9329_CMD_READ_MY_HID_DATA){
      
    }
  }
  
}
