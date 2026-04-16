using System;

namespace GrabApp.Core
{
    public interface IGrabPay
    {
        bool ProsesPembayaran(decimal nominal);
        string DapatkanStatusPembayaran();
    }

    public abstract class GrabService : IGrabPay
    {
        public string BookingId { get; set; }
        public string ServiceName { get; set; }

        public GrabService(string id, string name)
        {
            BookingId = id;
            ServiceName = name;
        }

     
        public abstract decimal HitungTarif();

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
    public class Kendaraan
    {
        public string PlatNomor { get; set; }
        public string TipeMobil { get; set; }
        public Kendaraan(string plat, string tipe) { PlatNomor = plat; TipeMobil = tipe; }
    }

    public class MitraGrab
    {
        public string NamaMitra { get; set; }
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

    
    public class Booking
    {
        public GrabService Layanan { get; set; }
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