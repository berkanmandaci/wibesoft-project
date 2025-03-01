# WibeSoft Çiftlik Oyunu

## 📖 Proje Hakkında

WibeSoft Çiftlik Oyunu, Unity ile geliştirilmiş modern bir çiftlik simülasyon oyunudur. Oyuncular kendi çiftliklerini yönetebilir, ekinler yetiştirebilir ve kaynaklarını verimli bir şekilde kullanarak ilerleyebilirler.

## 🎮 Temel Özellikler

- Grid tabanlı tarla sistemi
- Gerçek zamanlı ekin büyüme mekanizması
- Offline progress sistemi
- Envanter yönetimi
- Ekonomi sistemi (Altın ve Gem)
- Sürükle-bırak bina yerleştirme sistemi

## 🏗️ Sistem Mimarisi

Proje, MVC (Model-View-Controller) tasarım desenini temel alan modüler bir mimariye sahiptir.

### 📦 Proje Yapısı

```
Assets/_Project/
├── Art/                    # Görsel içerikler
│   ├── Materials/         # Materyal dosyaları
│   ├── Shaders/          # Shader dosyaları
│   └── Sprites/          # Sprite dosyaları
│
├── Prefabs/                # Prefab dosyaları
│
├── Scenes/                 # Sahne dosyaları
│
└── Scripts/               # Kod dosyaları
    ├── Core/            # Temel sistemler
    │   ├── Bootstrap/   # Başlatma sistemi
    │   │   ├── GameBootstrap.cs
    │   │   └── GameEvents.cs
    │   └── Managers/    # Singleton yöneticiler
    │       ├── GridManager.cs
    │       ├── CropManager.cs
    │       └── PlayerManager.cs
    │
    ├── Data/            # Veri modelleri
    │   ├── Models/      # Model sınıfları
    │   │   ├── PlayerData.cs
    │   │   └── CropData.cs
    │   └── ScriptableObjects/  # Yapılandırma dosyaları
    │       ├── CropConfig.cs
    │       └── GameConfig.cs
    │
    ├── Features/        # Oyun özellikleri
    │   ├── Grid/       # Grid sistemi
    │   │   ├── Cell.cs
    │   │   └── GridInteractionController.cs
    │   ├── Crops/      # Ekin sistemi
    │   │   └── CropGrowthController.cs
    │   └── Building/   # Bina sistemi
    │       └── DraggableObject.cs
    │
    └── UI/             # UI kodları
        ├── Views/      # UI görünüm sınıfları
        └── Controllers/ # UI kontrolcüleri
```

### 🎯 MVC Yapısı

#### Model (Data/)
- **PlayerData**: Oyuncu seviyesi, para, tecrübe puanı
- **CropData**: Ekin türü, büyüme durumu, ekim zamanı
- **GridData**: Hücre tipleri, pozisyonlar
- **InventoryData**: Envanter içeriği, miktarlar

#### View (UI/)
- **CropInfoView**: Ekin detayları ve büyüme durumu
- **InventoryView**: Envanter arayüzü
- **LevelView**: Seviye ve tecrübe gösterimi
- **WalletView**: Para ve gem gösterimi

#### Controller (Features/)
- **CropController**: Ekin ekme, büyütme, hasat işlemleri
- **GridController**: Grid etkileşimleri ve hücre yönetimi
- **InventoryController**: Envanter işlemleri
- **BuildingController**: Bina yerleştirme ve yönetimi

### 🔄 Veri Akışı ve Haberleşme

1. **Bootstrap Süreci**
   ```
   GameBootstrap → Manager'ların Başlatılması → Servislerin Başlatılması → UI Başlatılması
   ```

2. **Event Sistemi**
   ```
   Controller → GameEvents → İlgili Manager'lar → View Güncellemesi
   ```

3. **Veri Yönetimi**
   ```
   JsonDataService ←→ Manager'lar ←→ Controller'lar ←→ View'lar
   ```

### 🛠️ Temel Sistemler

1. **Manager Sistemi**
   - Singleton pattern ile merkezi yönetim
   - Her manager kendi sorumluluğuna sahip
   - Örnek: GridManager, CropManager, PlayerManager

2. **Event Sistemi**
   - Loosely coupled haberleşme
   - GameEvents üzerinden merkezi event yönetimi
   - Performans optimizasyonlu event handling

3. **Servis Katmanı**
   - JsonDataService: Veri kayıt/yükleme işlemleri
   - CropService: Ekin konfigürasyonları
   - AudioService: Ses yönetimi

4. **UI Sistemi**
   - Prefab tabanlı modüler UI
   - View-Controller eşleşmesi
   - Event-driven güncellemeler

## 📸 Oyun Görselleri

![Oyun Ekranı](Assets/_Project/Art/Screenshots/gameplay.png)
![Envanter](Assets/_Project/Art/Screenshots/inventory.png)
![Ekin Sistemi](Assets/_Project/Art/Screenshots/farming.png)

## 📦 Kullanılan Assetler

### Temel Assetler
1. **UniTask**
   - Asenkron işlemler için
   - Version: 2.3.3
   - [GitHub](https://github.com/Cysharp/UniTask)

2. **DOTween**
   - Animasyonlar için
   - Version: 1.2.7
   - [Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)

3. **Quick Outline**
   - Obje seçim efektleri için
   - Version: 1.0.3
   - [Asset Store](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488)

## 📝 Geliştici Notları

- Tüm geliştirmeler `Assets/_Project` altında yapılmalıdır
- Kod yazım kuralları için `Document/CodingGuidelines.md` dosyasına bakın
- Mimari detaylar için `Document/Architecture.md` dosyasına bakın

