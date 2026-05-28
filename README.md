# PingWatch 📡
PingWatch, kurumsal ağ altyapısındaki cihazların (sunucu, router, switch, kamera vb.) durumlarını gerçek zamanlı olarak izlemenizi sağlayan, hafif ve performanslı bir açık kaynak ağ izleme (Network Monitoring) sistemidir.
Endüstriyel standartlarda, temiz mimari (Clean Architecture) prensiplerine sadık kalınarak .NET 10 ile geliştirilmiştir.
## 🚀 Özellikler
- **Gerçek Zamanlı Ağ İzleme:** Arka planda çalışan entegre worker service ile cihazlara periyodik ping atar.
- **Modern & Kurumsal UI:** Açık tema, endüstriyel "Split-Screen" giriş paneli ve "Admin Dashboard" yerleşimi. (Herhangi bir ekstra CSS framework'ü gerektirmez, Vanilla CSS ve JS ile yazılmıştır).
- **Ağ Topolojisi Matrisi (Heatmap):** Sisteminizin genel sağlık durumunu anlık renk kodlu matris haritasından izleyin.
- **JWT Kimlik Doğrulama:** Bearer token tabanlı, güvenli oturum yönetimi. Rol tabanlı erişim kontrolü (Admin / Viewer).
- **Hibrit Şifreleme (BCrypt + SHA256):** Parolalar modern standart olan BCrypt ile tuzlanarak saklanır. (Eski sistemlerden geçiş için SHA256 legacy desteği içerir).
- **E-Posta Uyarıları (SMTP):** Bir cihaz çevrimdışı olduğunda (Offline), belirlediğiniz yönetici mail adreslerine anında uyarı e-postası gönderilir.
- **SQLite & EF Core:** Kurulum gerektirmeyen, taşınabilir ve hızlı SQLite veritabanı altyapısı.
## 🛠️ Teknolojiler
- **Backend:** C# / .NET 10 (Web API)
- **Mimari:** N-Tier / Clean Architecture (Core, Application, Infrastructure, Web)
- **Veritabanı:** SQLite & Entity Framework Core
- **Güvenlik:** JWT (JSON Web Tokens), BCrypt.Net-Next
- **Frontend:** HTML5, Vanilla JavaScript, Vanilla CSS (CSS Variables)
## 📦 Kurulum ve Çalıştırma
Proje çalıştırıldığında veritabanı dosyası (`pingwatch.db`) otomatik olarak oluşturulacak ve varsayılan kullanıcılar eklenecektir.
### Ön Koşullar
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) kurulu olmalıdır.
### Adımlar
1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/KULLANICI_ADINIZ/PingWatch.git
   cd PingWatch/PingWatch
   ```
2. Gerekli paketleri indirin:
   ```bash
   dotnet restore
   ```
3. Veritabanını oluşturun (Entity Framework Migration):
   ```bash
   dotnet ef database update
   ```
   *(Eğer dotnet-ef aracı kurulu değilse: `dotnet tool install --global dotnet-ef` komutu ile kurabilirsiniz)*
4. Projeyi çalıştırın:
   ```bash
   dotnet run
   ```
5. Tarayıcınızda şu adrese gidin:
   ```
   https://localhost:7153
   ```
### 🔑 Varsayılan Giriş Bilgileri
|
 Rol 
|
 Kullanıcı Adı 
|
 Şifre 
|
|
---
|
---
|
---
|
|
**
Yönetici (Admin)
**
|
`admin`
|
`admin123`
|
|
**
İzleyici (Viewer)
**
|
`viewer`
|
`viewer123`
|
*(Güvenliğiniz için kurulumdan sonra Yönetim Paneli üzerinden bu şifreleri değiştirin veya yeni kullanıcılar ekleyip eskilerini silin.)*
## 📁 Proje Mimarisi
Proje **Solid** ve **Clean Code** prensiplerine göre yapılandırılmıştır:
- **`Core/`**: Veritabanı varlıkları (Entities), DTO'lar, arayüzler (Interfaces) ve ortak Result sınıfları. Dışarıya hiçbir bağımlılığı yoktur.
- **`Application/`**: İş mantığının (Business Logic) bulunduğu servisler (`UserService`, `DeviceService`, `PingBackgroundService`).
- **`Infrastructure/`**: Veritabanı bağlamı (`AppDbContext`), Repository sınıfları (`DeviceRepository` vb.), Şifreleme algoritmaları ve dış servisler (SMTP).
- **`Controllers/`**: HTTP isteklerini karşılayan ince ve temiz REST API uç noktaları.
## 🤝 Katkıda Bulunma
Pull request'ler memnuniyetle kabul edilir. Büyük değişiklikler için lütfen önce neyi değiştirmek istediğinizi tartışmak için bir issue açın.
## 📄 Lisans
Bu proje [MIT Lisansı](LICENSE) altında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına bakabilirsiniz.
