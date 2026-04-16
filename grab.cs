using GrabApp.Core;
using System;

namespace GrabApp.Core
{
    // ==========================================
    // MATERI PERTEMUAN 07 (Abstraksi & Interface)
    // ==========================================

    // 1. INTERFACE
    // Kontrak standar untuk semua layanan Grab agar mendukung pembayaran digital (OVO/Kartu).
    public interface IGrabPay
    {
        bool ProsesPembayaran(decimal nominal);
        string DapatkanStatusPembayaran();
    }

    // 2. ABSTRACT 
    // Kelas dasar untuk semua layanan (GrabRide, GrabFood, GrabExpress).
    // Tidak bisa dibuat objeknya secara langsung (harus lewat kelas turunan).
    public abstract class GrabService : IGrabPay
    {
        public string BookingId { get; set; }
        public string ServiceName { get; set; }

        public GrabService(string id, string name)
        {
            BookingId = id;
            ServiceName = name;
        }

        // Abstract Method: Setiap layanan punya formula tarif yang berbeda.
        public abstract decimal HitungTarif();

        // Implementasi dari Interface IGrabPay
        public bool ProsesPembayaran(decimal nominal)
        {
            Console.WriteLine($"[OVO/GrabPay] Saldo terpotong Rp{nominal} untuk layanan {ServiceName}.");
            return true;
        }

        public string DapatkanStatusPembayaran() => "Success";
    }

    // KELAS TURUNAN KONKRET (Implementasi Abstract Class)
    public class GrabRide : GrabService
    {
        public double JarakTempuhKm { get; set; }
        private const decimal TarifPerKm = 3500;
        private const decimal BiayaDasar = 4000; // Flag fall / biaya minimum

        public GrabRide(string id, double jarak) : base(id, "GrabRide")
        {
            JarakTempuhKm = jarak;
        }

        public override decimal HitungTarif()
        {
            return BiayaDasar + ((decimal)JarakTempuhKm * TarifPerKm);
        }
    }


    // ==========================================
    // MATERI PERTEMUAN 06 (Relasi Antar Objek)
    // ==========================================

    // Kelas Pendukung
    public class Kendaraan
    {
        public string PlatNomor { get; set; }
        public string TipeMobil { get; set; }
        public Kendaraan(string plat, string tipe) { PlatNomor = plat; TipeMobil = tipe; }
    }

    // Kelas Mitra (Driver)
    public class MitraGrab
    {
        public string NamaMitra { get; set; }

        // 3. AGREGASI (Has-A Relationship, Weak Tie)
        // MitraGrab "punya" Kendaraan. Tapi jika akun Mitra dihapus/diblokir dari sistem,
        // objek mobil/motor di dunia nyata tetap ada dan tidak ikut hancur.
        public Kendaraan KendaraanOperasional { get; set; }

        public MitraGrab(string nama, Kendaraan kendaraan)
        {
            NamaMitra = nama;
            KendaraanOperasional = kendaraan;
        }

        public void TerimaOrder(string namaPenumpang)
        {
            Console.WriteLine($"[MITRA] {NamaMitra} menuju titik jemput {namaPenumpang} dengan {KendaraanOperasional.TipeMobil} ({KendaraanOperasional.PlatNomor}).");
        }
    }

    // Kelas E-Receipt (Resi Elektronik)
    public class EReceipt
    {
        public string Rincian { get; set; }
        public DateTime WaktuTerbit { get; set; }

        public EReceipt(string rincian)
        {
            Rincian = rincian;
            WaktuTerbit = DateTime.Now;
        }
    }

    // Kelas Booking (Pemesanan)
    public class Booking
    {
        public GrabService Layanan { get; set; }

        // 4. KOMPOSISI (Part-Of Relationship, Strong Tie)
        // Booking "terdiri dari" EReceipt. Resi elektronik ini di-instansiasi DI DALAM
        // pemesanan. Jika pemesanan dibatalkan (objek hancur), resi tidak akan pernah terbit.
        private EReceipt _receipt;

        public Booking(GrabService layanan)
        {
            Layanan = layanan;
            // Instansiasi ketat (Strong tie)
            _receipt = new EReceipt($"ID: {layanan.BookingId} | Layanan: {layanan.ServiceName}");
        }

        public void SelesaikanPerjalanan()
        {
            decimal totalTarif = Layanan.HitungTarif();
            Layanan.ProsesPembayaran(totalTarif);

            Console.WriteLine($"[E-RECEIPT] {_receipt.Rincian} | Total Tagihan: Rp{totalTarif} | Diterbitkan: {_receipt.WaktuTerbit}");
        }
    }

    // Kelas Penumpang (User Aplikasi)
    public class Penumpang
    {
        public string Nama { get; set; }

        public Penumpang(string nama) { Nama = nama; }

        // 5. ASOSIASI (Uses-A Relationship)
        // Penumpang "menggunakan" jasa MitraGrab. Keduanya adalah entitas mandiri yang 
        // berdiri sendiri, tapi berinteraksi (berkirim pesan/memanggil metode) saat order terjadi.
        public void PesanKendaraan(MitraGrab mitra, GrabRide layanan)
        {
            Console.WriteLine($"\n[PENUMPANG] {Nama} membuat pesanan {layanan.ServiceName}...");

            // Asosiasi terjadi di sini (Penumpang berinteraksi dengan Mitra)
            mitra.TerimaOrder(Nama);

            // Menjalankan proses yang di dalamnya ada Komposisi
            Booking perjalanan = new Booking(layanan);
            perjalanan.SelesaikanPerjalanan();
        }
    }
}



namespace GrabApp.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Setup Entitas yang Berdiri Sendiri
            Kendaraan mobilAvanza = new Kendaraan("B 8899 GRS", "Toyota Avanza");
            Penumpang penumpangRina = new Penumpang("Rina");

            // 2. Terjadi Agregasi (Mobil di-assign ke Mitra)
            MitraGrab mitraJoko = new MitraGrab("Pak Joko", mobilAvanza);

            // 3. Polimorfisme menggunakan Abstract Class
            // Rina memesan GrabRide dengan jarak 8.5 KM
            GrabRide pesananRide = new GrabRide("GRB-2023X99", 8.5);

            // 4. Terjadi Asosiasi & Komposisi
            // Rina (Penumpang) berinteraksi dengan Pak Joko (Mitra)
            penumpangRina.PesanKendaraan(mitraJoko, pesananRide);

            Console.ReadLine();
        }
    }
}