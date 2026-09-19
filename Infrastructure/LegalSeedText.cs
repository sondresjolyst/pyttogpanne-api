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

Appen og disse sidene drives av:

- **Selskap:** Sjølyst Innovation AS
- **Organisasjonsnummer:** 938 517 789
- **Adresse:** Mårvegen 21a, 4347 Lye

Oppskriftene er laget av Pyttogpanne, som også står bak
[@pyttogpanne](https://www.instagram.com/pyttogpanne/) på Instagram.

## 2. Aksept av vilkårene

Ved å bruke Pyttogpanne godtar du disse vilkårene. Er du uenig, bør du ikke bruke tjenesten.

## 3. Bruk av tjenesten

Pyttogpanne viser oppskrifter på turmat laget i én panne. Oppskriftene leses i appen, og du trenger
ingen konto.

## 4. Immaterielle rettigheter

Alt innhold i tjenesten, tekst, bilder og oppskrifter, eies av Pyttogpanne. Innholdet kan ikke
kopieres, publiseres på nytt eller brukes i egen markedsføring uten skriftlig samtykke fra oss.

## 5. Matlaging på eget ansvar

Oppskriftene er skrevet for matlaging ute, ofte med gassbrenner eller åpen ild. Du har selv ansvar
for trygg bruk av utstyret, for å følge lokale regler om bålbrenning, og for å vurdere innholdet opp
mot allergier og intoleranser.

## 6. Innhold merket som reklame

Innhold merket «Reklame» er laget etter at Pyttogpanne har mottatt produkter, rabatt eller betaling.
Merkingen følger kravene fra Forbrukertilsynet.

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

Vi kan oppdatere vilkårene. Det er versjonen som står her, med datoen nederst, som gjelder.

## 11. Kontakt

Spørsmål om vilkårene? Ta kontakt via [@pyttogpanne](https://www.instagram.com/pyttogpanne/) på
Instagram.
""";

        private const string PrivacyNo = """
## Behandlingsansvarlig

Sjølyst Innovation AS er behandlingsansvarlig for personopplysninger som samles inn gjennom appen og
disse sidene.

## Hvilke opplysninger vi samler inn

- **Favoritter og handleliste:** lagres på telefonen din. De sendes ikke til oss, og vi har ingen
  tilgang til dem.
- **Brukerkontoer:** navn, e-postadresse og passord (lagret som hash).
- **Bruksdata:** enkle tjenerlogger (IP-adresse, forespurte adresser) til feilsøking og sikkerhet.

Vi samler ikke inn betalingsopplysninger, og vi bruker ingen analyse- eller sporingsverktøy.

## Hva vi bruker opplysningene til

- Å sende e-post knyttet til innlogging (invitasjon, tilbakestilling av passord).
- Å forbedre tjenesten og finne tekniske feil.

Vi bruker ikke opplysningene til markedsføring, og vi selger dem ikke videre.

## Behandlingsgrunnlag

Vi behandler personopplysninger på følgende grunnlag (personvernforordningen artikkel 6):

- **Avtale (art. 6 nr. 1 bokstav b):** kontoopplysninger og e-post om innlogging er nødvendige for å
  gi tilgang til administrasjonssidene.
- **Berettiget interesse (art. 6 nr. 1 bokstav f):** tjenerlogger oppbevares for sikkerhet og
  feilsøking. Interessen vår går ikke foran rettighetene dine, og du kan protestere når som helst.

## Deling av opplysninger

Vi selger ikke personopplysninger. Tjenesten driftes på infrastruktur vi styrer selv, uten
skyleverandør bak. Vi bruker én databehandler:

- [Brevo](https://www.brevo.com/legal/termsofuse/) (Frankrike, EØS): utsending av e-post om
  innlogging. E-postadressen til den som har konto deles med Brevo kun til dette formålet.

Vi kan i tillegg utlevere opplysninger når loven krever det.

## Lagringstid

- **Kontoopplysninger:** til kontoen slettes, hvorpå navn, e-post og passord fjernes.
- **Tjenerlogger:** 90 dager.
- **Sikkerhetskopier av databasen:** krypterte. Etter sletting kan rester ligge i sikkerhetskopier i
  inntil omtrent 6 måneder, til rotasjonen er fullført. Sikkerhetskopier brukes ikke til behandling.

## Rettighetene dine

Etter personvernforordningen har du rett til:

- **Innsyn og dataportabilitet:** ta kontakt for en kopi av opplysningene vi har om deg.
- **Sletting:** ta kontakt for å få opplysningene dine slettet.
- **Retting:** ta kontakt hvis noe er feil.
- **Protest:** du kan protestere mot behandling som bygger på berettiget interesse.

Henvendelser sendes via [@pyttogpanne](https://www.instagram.com/pyttogpanne/) på Instagram. Du kan
også klage til Datatilsynet.

## Informasjonskapsler

Appen bruker ingen informasjonskapsler. På nett brukes de til innlogging. Se
[informasjonskapsler](/no/cookies) for detaljer.
""";

        private const string CookiesNo = """
## Hva er informasjonskapsler?

Informasjonskapsler er små tekstfiler som lagres i nettleseren din. De lar en nettside huske økten
din mellom forespørsler.

## I appen

Appen bruker ingen informasjonskapsler. Oppskrifter, favoritter og handleliste lagres på telefonen,
slik at appen virker uten dekning.

## Kapslene vi bruker på nett

Å lese disse sidene setter ingen kapsler for sporing eller annonser. Kapslene under settes bare ved
innlogging.

| Navn | Formål | Varighet |
| --- | --- | --- |
| `next-auth.session-token` | Holder deg innlogget | Økt eller 30 dager |
| `next-auth.csrf-token` | Sikkerhet, hindrer forfalskning av forespørsler på tvers av nettsteder | Økt |
| `next-auth.callback-url` | Husker hvor du skal sendes etter innlogging | Økt |

## Tredjeparter

Vi bruker ingen kapsler fra tredjeparter til analyse, annonsering eller sporing.

## Hvordan styre kapsler

Du kan slette eller blokkere informasjonskapsler i nettleserinnstillingene. Å blokkere kapslene over
påvirker innlogging.

## Spørsmål?

Ta kontakt via [@pyttogpanne](https://www.instagram.com/pyttogpanne/) på Instagram.
""";
    }
}
