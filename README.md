<div align="center">

# ✈️ DHMİ - Olay ve Bakım Takip Sistemi (OBTS)

[![C#](https://img.shields.io/badge/C%23-.NET%208-512BD4?style=for-the-badge&logo=c-sharp&logoColor=white)](#)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](#)
[![MSSQL](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)](#)
[![Bootstrap 5](https://img.shields.io/badge/Bootstrap%205-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](#)

 Devlet Hava Meydanları İşletmesi (DHMİ) Esenboğa Havalimanı Başmüdürlüğü staj dönemi projesi.

</div>

---

## 📌 Proje Hakkında
**DHMİ Olay ve Bakım Takip Sistemi (OBTS)**; havalimanı terminali içerisinde meydana gelen teknik arıza, olay ve bakım süreçlerinin dijital ortamda şeffaf, hızlı ve güvenli bir şekilde takip edilmesini sağlayan kurumsal bir web tabanlı otomasyon sistemidir[cite: 1]. 

Uygulama; havalimanı personelinin karşılaştığı teknik sorunları (örneğin apron aydınlatma, yürüyen merdiven veya donanım arızaları) konum, BİM (Demirbaş) kodu ve detay bilgileriyle birlikte sisteme bildirmesine, yetkili teknikerlerin ve yöneticilerin bu süreçleri uçtan uca yönetmesine olanak tanır.

---

## 🚀 Temel Özellikler ve Modüller

### 1. Rol Bazlı Kimlik Doğrulama ve Güvenlik (ASP.NET Core Identity)
* **Hiyerarşik Yetkilendirme:** Sistemde **Yönetici**, **Tekniker** ve **Personel** olmak üzere 3 temel rol tanımlanmıştır.
* **Yönetici Onay Mekanizması:** Yeni kayıt olan kullanıcılar otomatik olarak sisteme erişemez; yönetici onayı ve rol ataması sonrasında aktifleşir.
* **Tek Kullanımlık Geçici Şifre (OTP):** Şifresini unutan kullanıcılar için BT departmanı tarafından rastgele 6 haneli geçici şifre üretilir ve ilk girişte zorunlu şifre değiştirme ekranına yönlendirilir.
* **Güvenlik Tedbirleri:** Form geçişlerinde CSRF saldırılarına karşı `[ValidateAntiForgeryToken]` koruması ve yüklenen görsellerde katı uzantı (`.jpg`, `.png`) kontrolleri yer alır.

### 2. Personel Arıza Bildirim Paneli
* Personeller, geçmişte açtıkları tüm arıza kayıtlarını listeleyebilir ve güncel durumlarını ("Tekniker Alınmayı Bekliyor", "Tamir Ediliyor", "Firmaya Gönderildi", "Kişi Onayında", "İşlem Bitti") şeffaf bir şekilde takip edebilir.
* Çözülen arızalar için personelin sistem üzerinden "Teslim Aldım / Onayla" butonuyla süreci kapatması sağlanır.

### 3. Tekniker Kontrol Paneli & Canlı Operasyon
* **Dinamik Filtreleme:** BİM No, Arıza No, Kategori (Donanım/Yazılım), Öncelik Derecesi ve "Uçuş Operasyonunu Etkiliyor mu?" kriterlerine göre gelişmiş arıza süzme algoritmaları.
* **Asenkron Canlı Veri Akışı:** Havalimanı kriz yönetimi standartlarına uygun olarak sayfanın arka planda her 30 saniyede bir güncellenmesi (form doldurma esnasında veri kaybını önlemek için aktif modal pencerelerinde yenilemenin askıya alınması).
* **Firma Yönlendirme Takibi:** Dış servise gönderilen cihazlar için özel çağrı açılış detayları, kullanılan yedek parça ve harici firma müdahale süresi sayaçları.

### 4. Yönetici Dashboard ve Performans İzleme
* Sistemdeki toplam arıza, acil öncelikli vakalar, firmada bekleyenler ve çözülen işlemler anlık istatistik kartlarıyla izlenir.
* **Tekniker Performans Modülü:** LINQ `GroupBy` sorguları ile hangi teknikerin kaç arızaya müdahale ettiği ve kaçını çözüme kavuşturduğu raporlanır.

---

## 🛠️ Teknik Altyapı ve Mimari

| Katman / Teknoloji | Kullanılan Araçlar & Kütüphaneler |
| :--- | :--- |
| **Backend** | C#, ASP.NET Core MVC (.NET 8)[cite: 1] |
| **ORM & Veritabanı** | Entity Framework Core (Code-First yaklaşımı), Microsoft SQL Server (MSSQL)[cite: 1] |
| **Güvenlik** | ASP.NET Core Identity (Authentication & Authorization) |
| **Frontend** | HTML5, CSS3, Bootstrap 5, JavaScript (AJAX / DOM Manipülasyonu) |

---

## 📂 Veritabanı Mimarisi (Code-First)
Projede ilişkisel veri bütünlüğünü sağlamak amacıyla şu temel modeller oluşturulmuştur:
* **Arizalar:** Arıza başlığı, açıklama, bildirim/çözüm tarihleri, BİM kodu, öncelik derecesi, uçuş operasyon etkisi ve durum bilgileri.
* **Konumlar:** Havalimanı içerisindeki birimlerin ve lokasyonların tutulduğu tablo (Foreign Key ile ilişkilendirilmiştir).
* **ArizaIslemGecmisleri:** Teknikerlerin arızaya yaptığı müdahalelerin ve işlem geçmişi loglarının tutulduğu tablo.
* **KullaniciEkBilgileri:** Standart `IdentityUser` yapısını bozmadan hesap onay durumunu (`IsApproved`) ve geçici şifre bayrağını (`IsTemporaryPassword`) tutan 1-to-1 ilişki tablosu.

---

## ⚙️ Kurulum ve Çalıştırma

1. Repoyu bilgisayarınıza klonlayın:
   ```bash
   git clone [https://github.com/KULLANICI_ADINIZ/DHM_OBTS.git](https://github.com/KULLANICI_ADINIZ/DHM_OBTS.git)
2. Projeyi Visual Studio ile açın.

3. appsettings.json dosyası içerisindeki ConnectionStrings alanını kendi MSSQL Server (SQLEXPRESS) bağlantı adresinize göre güncelleyin.

4. Paket Yöneticisi Konsolu (Package Manager Console) üzerinden veritabanını oluşturmak için migration komutunu çalıştırın:
   ```bash
   Update-Database
5. Projeyi derleyin ve IIS Express veya Kestrel üzerinden çalıştırın. (İlk çalıştırmada DbInitializer sınıfı sayesinde varsayılan roller ve yönetici hesabı otomatik olarak oluşturulacaktır).

👨‍💻 Geliştirici
Mustafa Aslan - Bilgisayar Mühendisi
