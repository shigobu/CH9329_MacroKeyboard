#include <ResKeypad.h>
#include "CH9329_Keyboard.h"

// ボーレート設定値計算マクロ。最後の"+0.5"は、四捨五入させるため？？
#define USART0_BAUD_RATE(BAUD_RATE) ((float)(F_CPU * 64 / (16 * (float)BAUD_RATE)) + 0.5)

#define SW_OFF HIGH
#define SW_ON LOW

const int SW_PIN = PIN_PA1;
const int SW_ON_THRESHOLD = 10;
uint8_t reportData[KEY_REPORT_DATA_LENGTH] = {};

const int KeyNum = 7;
PROGMEM const int threshold[KeyNum] = {
  // 次の数列は、しなぷすのハード製作記の回路設計サービスで計算して得られたもの
  42, 165, 347, 511, 641, 807, 965
};
ResKeypad keypad(SW_PIN, KeyNum, threshold);

// USART初期化
void USART0_init(void) {
  PORTA.DIR &= ~PIN7_bm;
  PORTA.DIR |= PIN6_bm;
  USART0.BAUD = (uint16_t)USART0_BAUD_RATE(CH9329_DEFAULT_BAUDRATE);
  USART0.CTRLB |= USART_TXEN_bm;
  USART0.CTRLB |= USART_RXEN_bm;
}

// 一つの値送信
void USART0_sendValue(uint8_t c) {
  while (!(USART0.STATUS & USART_DREIF_bm)) {
    ;
  }
  USART0.TXDATAL = c;
}

//一つの値受信
uint8_t USART0_read()
{
  while (!(USART0.STATUS & USART_RXCIF_bm)) {
    ;
  }
  return USART0.RXDATAL;
}

// 複数の値送信
void USART0_sendValue(uint8_t* c, size_t length) {
  for (size_t i = 0; i < length; i++ ) {
    USART0_sendValue(c[i]);
  }
}

// CH9329へキー押下情報送信
void CH9329_write(uint8_t c){
  size_t length = 0;
  CH9329_Keyboard.press(c);
  length = CH9329_Keyboard.getReportData(reportData, KEY_REPORT_DATA_LENGTH);
  USART0_sendValue(reportData, length);

  CH9329_Keyboard.release(c);
  length = CH9329_Keyboard.getReportData(reportData, KEY_REPORT_DATA_LENGTH);
  USART0_sendValue(reportData, length);
}

void setup() {
  USART0_init();
  CH9329_Keyboard.begin();

  pinMode(SW_PIN, INPUT);
  delay(5000);
}

void loop() {
  
}
