namespace pyttogpanne_api.Infrastructure
{
    /// <summary>
    /// Starting text for the legal pages, written once on an empty database. An admin edits
    /// these from /admin/legal afterwards; the seeder never overwrites an existing page.
    /// </summary>
    public static class LegalSeedText
    {
        public record Page(string Key, string Locale, string Title, string Body);

        public static readonly Page[] All =
        [
            new("terms", "no", "Vilkår", TermsNo),
            new("privacy", "no", "Personvern", PrivacyNo),
            new("cookies", "no", "Informasjonskapsler", CookiesNo),
        ];

        private const string TermsNo = """
## 1. Om oss

Denne nettsiden og appen blir drivne av Pyttogpanne.
Kontaktopplysningar blir fylte ut av Pyttogpanne i admin.

## 2. Aksept av vilkårene

Ved å bruke Pyttogpanne godtar du disse vilkårene. Er du uenig, bør du ikke bruke nettsiden.

## 3. Bruk av tjenesten

Pyttogpanne viser datamaskiner vi har bygget — både maskiner som er til salgs og maskiner som er
solgt — og lar deg ta kontakt for å få et tilbud. Det er bare vi som drifter nettsiden som kan logge
inn.

## 4. Immaterielle rettigheter

Alt innhold på siden — tekst, bilder, spesifikasjoner og navnet og logoen til Pyttogpanne — eies av
Pyttogpanne. Innholdet kan ikke kopieres, publiseres på nytt eller brukes i egen markedsføring uten
skriftlig samtykke fra oss.

## 5. Henvendelser og bestilling

Å sende inn kontaktskjemaet er en forespørsel, ikke en bindende avtale. Hva som skal leveres, hvilke
deler som inngår og hva det koster, avtaler du direkte med oss.

## 6. Priser og spesifikasjoner

Prisen og delelisten som står oppført på en datamaskin, gjelder maskinen slik den var satt sammen da
den ble lagt ut. Delepriser endrer seg, så en tilsvarende maskin kan koste noe annet i dag. En
datamaskin som er merket som solgt, er ikke lenger til salgs.

## 7. Tilgjengelighet

Vi tilstreber høy oppetid, men garanterer ikke uavbrutt tilgang. Tjenesten kan oppdateres eller tas
ned for vedlikehold.

## 8. Personvern

Se [personvernerklæringen](/no/privacy) for hvordan vi behandler personopplysninger, og
[informasjonskapsler](/no/cookies) for bruk av cookies.

## 9. Lovvalg

Norsk rett gjelder for disse vilkårene. Tvister som ikke lar seg løse direkte, kan bringes inn for
norske domstoler.

## 10. Endringer

Vi kan oppdatere vilkårene. Det er versjonen som står her, med datoen øverst, som gjelder.

## 11. Kontakt

Spørsmål om vilkårene? [Ta kontakt](/no/contact).
""";

        private const string PrivacyNo = """
## Behandlingsansvarlig

Pyttogpanne er behandlingsansvarlig for personopplysninger som samles inn gjennom denne nettsiden.

## Hvilke opplysninger vi samler inn

- **Henvendelser:** navn, e-post, telefonnummer, bruksområde, budsjett og meldingen du sender inn i
  kontaktskjemaet.
- **Brukerkontoer:** navn, e-postadresse og passord (lagret som hash). Kontoer opprettes kun for oss
  som drifter siden — det er ingen registrering for besøkende.
- **Bruksdata:** enkle tjenerlogger (anonyme bruker-ID-er, forespurte adresser) til feilsøking og
  sikkerhet.

Vi samler ikke inn betalingsopplysninger gjennom denne siden.

## Hva vi bruker opplysningene til

- Å svare på henvendelsen din og avtale arbeidet du spør om.
- Å sende e-post knyttet til innlogging (invitasjon, tilbakestilling av passord).
- Å forbedre tjenesten og finne tekniske feil.

Vi bruker ikke opplysningene til markedsføring, og vi selger dem ikke videre.

## Behandlingsgrunnlag

- **Avtale (art. 6 nr. 1 bokstav b)** — kontoopplysninger og e-post om innlogging er nødvendige for å
  gi tilgang til administrasjonssidene.
- **Berettiget interesse (art. 6 nr. 1 bokstav f)** — henvendelser behandles for at vi skal kunne
  svare deg, og tjenerlogger oppbevares for sikkerhet og feilsøking. Interessen vår går ikke foran
  rettighetene dine — du kan protestere når som helst.

## Deling av opplysninger

Vi selger ikke personopplysninger. Plattformen driftes på infrastruktur vi styrer selv; det er ingen
skyleverandør bak. Vi bruker én databehandler:

- [Brevo](https://www.brevo.com/legal/termsofuse/) (Frankrike, EØS) — utsending av e-post
  (invitasjon, tilbakestilling av passord) og videresending av henvendelser fra kontaktskjemaet.
  E-postadressen din og innholdet i e-posten deles med Brevo kun til dette formålet.

Vi kan i tillegg utlevere opplysninger når loven krever det.

## Lagringstid

- **Henvendelser** — så lenge det trengs for å håndtere forespørselen og eventuell oppfølging,
  deretter slettes de.
- **Kontoopplysninger** — til kontoen slettes, hvorpå navn, e-post og passord fjernes.
- **Tjenerlogger** — 90 dager. **Måledata** — 60 dager.
- **Sikkerhetskopier av databasen** — kryptert. Etter sletting kan rester ligge i sikkerhetskopier i
  inntil omtrent 6 måneder til rotasjonen er fullført. Sikkerhetskopier brukes ikke til behandling.

## Rettighetene dine

Etter personvernforordningen har du rett til:

- **Innsyn og dataportabilitet** — [ta kontakt](/no/contact) for en kopi av opplysningene vi har om
  deg.
- **Sletting** — [ta kontakt](/no/contact) for å få slettet henvendelsen din og opplysningene i den.
- **Retting** — [ta kontakt](/no/contact) hvis noe er feil.
- **Protest** — du kan protestere mot behandling som bygger på berettiget interesse.

Henvendelser sendes til kontaktadressa som er oppgitt i appen. Du kan også klage
til Datatilsynet.

## Informasjonskapsler

Vi bruker informasjonskapsler til innlogging og økter. Se
[informasjonskapsler](/no/cookies) for detaljer.
""";

        private const string CookiesNo = """
## Hva er informasjonskapsler?

Informasjonskapsler er små tekstfiler som lagres i nettleseren din. De lar en nettside huske økten
din mellom forespørsler.

## Kapslene vi bruker

Å surfe på siden setter ingen kapsler for sporing eller annonser. Kapslene under settes bare når noen
logger inn.

| Navn | Formål | Varighet |
| --- | --- | --- |
| `next-auth.session-token` | Holder deg innlogget | Økt / 30 dager |
| `next-auth.csrf-token` | Sikkerhet — hindrer forfalskning av forespørsler på tvers av nettsteder | Økt |
| `next-auth.callback-url` | Husker hvor du skal sendes etter innlogging | Økt |

## Tredjeparter

Vi bruker ingen kapsler fra tredjeparter til analyse, annonsering eller sporing.

## Hvordan styre kapsler

Du kan slette eller blokkere informasjonskapsler i nettleserinnstillingene. Å blokkere kapslene over
påvirker bare det å holde seg innlogget; resten av siden virker uten dem.

## Spørsmål?

[Ta kontakt](/no/contact) hvis du lurer på noe rundt bruken av informasjonskapsler.
""";
    }
}
