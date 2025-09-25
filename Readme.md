# 🎮 Thiên Hùng Ca  
### *Celestial Hymn: Legends of Vietnam*  

⚔️ Một **2D Platform Fighter** lấy cảm hứng từ **Brawlhalla**, tái hiện các **anh hùng huyền thoại Việt Nam** với phong cách nghệ thuật sống động, gameplay mượt mà và hệ thống vũ khí đa dạng.  

---

## 🌟 Thông Tin Dự Án
- 🏷️ **Tên gốc (VN):** Thiên Hùng Ca  
- 🌍 **Tên quốc tế (EN):** Celestial Hymn: Legends of Vietnam  
- 🛠️ **Engine:** Unity (6.000.05.01F LTS)  
- 💻 **Ngôn ngữ:** C#  
- 👥 **Đội ngũ phát triển:**  
  - 🧑‍💻 **Long** – Lead Developer  
  - 🎨 **Đạt** – Game Designer & Animation Lead  
  - 🖌️ **Phương** – Art & AI Pipeline Lead  
  - ⚙️ **Khang** – DevOps & Build Lead  

---

## 🚀 Tính Năng Chính
- 🎯 **Combat chuẩn eSports**: Responsive control, knockback damage system.  
- 🗡️ **Vũ khí linh hoạt**: Attach prefab vũ khí → 1 animation set dùng cho nhiều loại vũ khí.  
- 🌀 **Hệ thống kỹ năng**: Combo light / heavy, dodge, multi-jump, wall cling.  
- ✨ **VFX độc quyền**: Slash, aura, elemental effects.  
- 🤖 **Pipeline AI Asset**: Tạo nhanh concept, sprite sheet, animation & VFX.  
- 👥 **Multiplayer Mode**: 1v1, 2v2, FFA (local & online roadmap).  

---

## 📂 Cấu Trúc Repo
```bash
/Assets
  /Art
    /Characters
      /ThanhGiong
        idle.png
        run.png
        attack_combo.png
    /VFX
  /Prefabs
    /Characters
    /Weapons
    /UI
  /Scenes
    MainMenu.unity
    Arena_Test.unity
  /Scripts
    /Core
      GameManager.cs
      InputManager.cs
      DamageSystem.cs
    /Characters
      CharacterController2D.cs
      WeaponAttach.cs
    /UI
/Docs
README.md
LICENSE
🛠️ Công Nghệ & Công Cụ

🎮 Unity 2023 LTS + C#

🖼️ Art & Sprite: Aseprite, Piskel, Photoshop

🤖 AI Asset Tools: Stable Diffusion XL, Leonardo.AI, Scenario.com

🔥 VFX Tools: Unity Particle System, EmberGen

🌐 Multiplayer (roadmap): Photon Fusion / Mirror / Unity Netcode

✨ Hero Showcase (Prototype)

🐎 Thánh Gióng – cưỡi ngựa sắt, thương lửa 🔥

🐘 Hai Bà Trưng – cưỡi voi chiến, thương chùy ⚔️

⚡ Nguyễn Huệ (Quang Trung) – tốc chiến, sấm sét ⚡

🗡️ Trần Hưng Đạo – kiếm pháp, chiến lược thủy quân 🌊

📜 Roadmap

✅ Core movement & input system

✅ Damage & knockback prototype

✅ Weapon prefab attach system

🔄 Thánh Gióng hero sprite (Idle, Run, Attack)

🔄 VFX prototype (spear slash, aura)

⏳ Additional heroes (Hai Bà Trưng, Nguyễn Huệ, Trần Hưng Đạo)

⏳ Local multiplayer polish + UI/UX

⏳ Online netcode integration

⏳ Soundtrack & SFX hùng tráng

🤝 Hướng Dẫn Đóng Góp

🌱 Fork repo → tạo branch feature/<tên>

💻 Commit theo chuẩn:

feat(character): add spear attack for Thanh Giong
fix(input): resolve dodge delay issue


🔀 Tạo Pull Request → assign reviewer.

📜 License

📄 MIT License – tự do phát triển, phân phối & mở rộng.

🏆 Credits

Lấy cảm hứng từ: Brawlhalla, Super Smash Bros.

Công cụ AI: Stable Diffusion XL, Scenario, Leonardo.AI.

Âm nhạc & SFX (sẽ bổ sung credit cụ thể khi tích hợp).