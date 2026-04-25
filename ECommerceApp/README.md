# E-Commerce App - Yazılım Test ve Kalite Raporu

**Proje Konusu:** E-Ticaret Sepet ve Sipariş Süreçleri Test Otomasyonu
**Kullanılan Teknolojiler:** C#, NUnit (.NET 10.0)
**Amaç:** Sistemdeki mantıksal hataları (bug) farklı test teknikleriyle (White, Black, Gray, Integration) tespit etmek.

## Test Senaryoları ve Sonuç Raporu

Sistem üzerinde toplam 10 test senaryosu koşulmuştur. Testlerin 5'i beklenen senaryolarda başarılı (PASS) olmuş, 5'i ise sistemin içine kasıtlı olarak yerleştirilmiş mantıksal hatalara (bug) takılarak başarısız (FAIL) olmuştur.

### 1. Unit Test (White Box)
İç kod yapısı ve mantıksal dallanmalar (if/else) hedeflenerek yazılmıştır.
* **[PASS] TC01_WhiteBox_GetTotalPrice_Under100_CalculatesCorrectly:** 100 TL altı sepet tutarı doğru hesaplandı.
* **[FAIL] TC02_WhiteBox_GetTotalPrice_Over100_AppliesDiscountCorrectly:** * **Neden Başarısız?** Sistemde 100 TL üzerine indirim uygulanması beklenen kod bloğunda matematiksel bir hata mevcuttur. `%10` indirim hesaplamak yerine, tutardan direkt `100 TL` çıkarılmaktadır. Beklenen tutar 135 TL iken sistem 50 TL döndürmüştür.
* **[PASS] TC03_WhiteBox_CartClear_EmptiesItemList:** Sepeti temizleme fonksiyonu listeyi başarıyla boşalttı.

### 2. Black Box Test
Sistemin iç yapısı bilinmeden, sadece uç girdiler (edge cases) ve beklenen çıktılar test edilmiştir.
* **[FAIL] TC04_BlackBox_AddProduct_NegativeQuantity_ShouldThrowException:**
  * **Neden Başarısız?** Kullanıcı sepete `-2` adet ürün eklemeye çalıştığında sistemin `ArgumentException` fırlatması beklenmektedir. Ancak `Cart.AddProduct` metodunda eksi değer kontrolü (validation) olmadığı için sistem hatayı fırlatmamış, sessizce geçiştirmiştir.
* **[PASS] TC05_BlackBox_PlaceOrder_EmptyCart_ReturnsFalse:** Boş sepetle ödeme sayfasına geçiş doğru bir şekilde engellendi.
* **[FAIL] TC06_BlackBox_ProcessPayment_NegativeAmount_ShouldFail:**
  * **Neden Başarısız?** Ödeme servisi, negatif bir bakiye (`-50 TL`) gönderildiğinde işlemi reddetmelidir. Ancak ödeme servisinde negatif tutar kontrolü eksiktir ve işlem `True` (Başarılı) dönmektedir.

### 3. Gray Box Test
Veri durumu (state) ve dış davranış entegre edilerek kontrol edilmiştir.
* **[FAIL] TC07_GrayBox_PlaceOrder_InsufficientStock_ShouldNotGoBelowZero:**
  * **Neden Başarısız?** Sepete mevcut stoktan fazla ürün eklendiğinde, sistem siparişi onaylamakta ve nesnedeki stoğu eksiltmektedir. Bu durum stoğun eksi (`-1`) değerlere düşmesine sebep olmuştur. `OrderService` içinde stok yetersizlik kontrolü eksiktir.
* **[FAIL] TC08_GrayBox_PlaceOrder_SuccessfulPayment_ShouldClearCart:**
  * **Neden Başarısız?** Kullanıcı ödemeyi başarıyla tamamladıktan sonra oturumdaki sepetin temizlenmesi gerekir. Ancak `OrderService.PlaceOrder` metodu başarılı dönüş yapmasına rağmen `cart.Clear()` işlemini tetiklemeyi unutmuştur. Sepetteki ürün sayısı 0 olması gerekirken 1 kalmıştır.

### 4. Integration Test
Modüllerin (Cart ve OrderService) birbiriyle uyumlu çalışıp çalışmadığı test edilmiştir.
* **[PASS] TC09_Integration_EndToEnd_ValidOrderFlow_ReducesStockCorrectly:** Standart bir kullanıcı akışı (ürün ekle -> sipariş ver) modüller arası başarıyla gerçekleşti ve stoklar doğru miktarda düştü.
* **[PASS] TC10_Integration_EndToEnd_OrderWithDiscount_ShouldProcessProperly:** Sistem indirim algoritmasında hata yapsa bile, modüller arası veri iletişiminin kopmadığı ve hatalı tutarla da olsa sipariş sürecinin Payment servisine iletilip tamamlandığı doğrulanmıştır. (İşlem çökmeden tamamlandığı için Integration seviyesinde PASS vermiştir).

---
**Sonuç ve Aksiyon:** Sistemin canlıya alınmadan önce stok kontrol mekanizmalarının eklenmesi, sepet temizleme işlemlerinin tetiklenmesi, indirim algoritmasının düzeltilmesi ve negatif veri (adet/tutar) girişlerinin engellenmesi gerekmektedir.