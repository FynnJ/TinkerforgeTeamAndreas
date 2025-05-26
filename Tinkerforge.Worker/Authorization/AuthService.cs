using Tinkerforge;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Tinkerforge.Worker.Authorization;

public class AuthService : IDisposable
{
    private Timer _timer;
    private readonly int _timeoutMilliseconds = 30000;
    
    private readonly IPConnection _ipConnection;
    
    private BrickletNFCRFID _nfc;
    private BrickletEPaper296x128 _eInk;
    private BrickletRGBLEDButton _ledButton;
    
    private readonly string nfcUid = "22ND";
    private readonly string eInkUid = "XGL";
    private readonly string rgbUid = "23Qx";
    
    // Following HashSet contains only dummy values
    private readonly HashSet<string> _authorizedUids = new()
    {
        "04A1B2C3D4",
        "123456789A"
    };

    public AuthService(IPConnection ipConnection)
    {
        _ipConnection = ipConnection ?? throw new ArgumentNullException(nameof(ipConnection));
    }

    public bool Initialize()
    {
        try
        {
            _nfc = new BrickletNFCRFID(nfcUid, _ipConnection);
            _eInk = new BrickletEPaper296x128(eInkUid, _ipConnection);
            _ledButton = new BrickletRGBLEDButton(rgbUid, _ipConnection);
            
            _eInk.FillDisplay(BrickletEPaper296x128.COLOR_BLACK);
            
            _timer = new Timer(OnTimeout, null, Timeout.Infinite, Timeout.Infinite);

            _ipConnection.EnumerateCallback += OnEnumerate;
            _ipConnection.ConnectedCallback += OnConnected;
            _ipConnection.DisconnectedCallback += OnDisconnected;

            _nfc.StateChangedCallback += OnStateChanged;
            _ledButton.ButtonStateChangedCallback += OnButtonStateChanged;
            
            _nfc.RequestTagID(BrickletNFCRFID.TAG_TYPE_MIFARE_CLASSIC);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Initialisierung fehlgeschlagen: {ex.Message}");
            return false;
        }
    }

    private void OnButtonStateChanged(BrickletRGBLEDButton sender, byte state)
    {
        if (state == 0)
        {
            ResetAuthenticationState();
        }
    }
    
    private void ResetAuthenticationState()
    {
        _eInk.FillDisplay(BrickletEPaper296x128.COLOR_BLACK);
        _eInk.DrawText(16, 48, BrickletEPaper296x128.FONT_24X32, 
            BrickletEPaper296x128.COLOR_WHITE,
            BrickletEPaper296x128.ORIENTATION_HORIZONTAL, "Ready");
        _eInk.Draw();
    
        _ledButton.SetColor(0, 0, 255); // Blau für Bereitschaft
        
        _nfc.RequestTagID(BrickletNFCRFID.TAG_TYPE_MIFARE_CLASSIC);
    }

    private void OnStateChanged(BrickletNFCRFID sender, byte state, bool idle)
{
    if (state == BrickletNFCRFID.STATE_REQUEST_TAG_ID)
    {
        try
        {
            // UID holen
            sender.GetTagID(out byte tagType, out byte tidLength, out byte[] tid);
            string uid = BitConverter.ToString(tid, 0, tidLength).Replace("-", "");

            Console.WriteLine($"Karte erkannt: {uid}");

            if (_authorizedUids.Contains(uid))
            {
                Console.WriteLine($"Zugriff gestattet: {uid}");
                _eInk.DrawText(16, 48, BrickletEPaper296x128.FONT_24X32, 
                    BrickletEPaper296x128.COLOR_WHITE,
                    BrickletEPaper296x128.ORIENTATION_HORIZONTAL, "Hello User");
                _eInk.Draw();
                _ledButton.SetColor(0, 255, 0);
                
                // Timeout für Session-Ende setzen
                _timer.Change(_timeoutMilliseconds, Timeout.Infinite);
            }
            else
            {
                Console.WriteLine($"Zugriff verweigert: {uid}");
                _eInk.DrawText(16, 48, BrickletEPaper296x128.FONT_24X32, 
                    BrickletEPaper296x128.COLOR_WHITE,
                    BrickletEPaper296x128.ORIENTATION_HORIZONTAL, "Access Denied");
                _eInk.Draw();
                _ledButton.SetColor(255, 0, 0);
                
                _timer.Change(5000, Timeout.Infinite);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Auslesen der UID: {ex.Message}");
            _timer.Change(3000, Timeout.Infinite);
        }
    }
}
    
    private void OnConnected(IPConnection sender, short connectReason)
    {
        Console.WriteLine($"Verbindung hergestellt, Grund: {connectReason}");
        sender.Enumerate();
    }
    
    private void OnDisconnected(IPConnection sender, short disconnectReason)
    {
        Console.WriteLine($"Verbindung getrennt, Grund: {disconnectReason}");
        
        if (disconnectReason == IPConnection.DISCONNECT_REASON_ERROR)
        {
            TryReconnect();
        }
    }
    
    private void OnEnumerate(IPConnection sender, string uid, string connectedUid, 
        char position, short[] hardwareVersion, 
        short[] firmwareVersion, int deviceIdentifier, 
        short enumerationType)
    {
        if (enumerationType == IPConnection.ENUMERATION_TYPE_CONNECTED || 
            enumerationType == IPConnection.ENUMERATION_TYPE_AVAILABLE)
        {
            if (uid == nfcUid)
            {
                Console.WriteLine("NFC-Reader gefunden");
            }
            else if (uid == eInkUid)
            {
                Console.WriteLine("E-Paper-Display gefunden");
            }
            else if (uid == rgbUid)
            {
                Console.WriteLine("RGB-LED-Button gefunden");
            }
        }
    }
    
    private void TryReconnect(int maxAttempts = 5)
    {
        int attempts = 0;
        bool connected = false;
    
        while (!connected && attempts < maxAttempts)
        {
            attempts++;
            try
            {
                Console.WriteLine($"Wiederverbindungsversuch {attempts}/{maxAttempts}...");
            
                _ipConnection.Connect("localhost", 4223);
                connected = true;
            
                Console.WriteLine("Wiederverbindung erfolgreich");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wiederverbindung fehlgeschlagen: {ex.Message}");
                Thread.Sleep(2000);
            }
        }
    
        if (!connected)
        {
            Console.WriteLine("Maximale Anzahl von Wiederverbindungsversuchen erreicht");
        }
    }
    
    private void OnTimeout(object state)
    {
        Console.WriteLine("Timeout: Automatisches Zurücksetzen der Authentifizierung");
        ResetAuthenticationState();
    }

    public void Dispose()
    {
        try
        {
            _timer?.Dispose();
            
            if (_nfc != null) 
                _nfc.StateChangedCallback -= OnStateChanged;
        
            if (_ledButton != null)
                _ledButton.ButtonStateChangedCallback -= OnButtonStateChanged;
        
            if (_ipConnection != null)
            {
                _ipConnection.EnumerateCallback -= OnEnumerate;
                _ipConnection.ConnectedCallback -= OnConnected;
                _ipConnection.DisconnectedCallback -= OnDisconnected;
            }
        
            _ledButton?.SetColor(0, 0, 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler beim Freigeben der Ressourcen: {ex.Message}");
        }
    }
}