# Welcome to Walking Dinner

## Plan van aanpak
* We zijn begonnen met het opzoeken van tutorials over de technieken die we wilden gaan gebruiken ( ASP.NET Core, Razor )
* We hebben gezocht naar voorbeelden van een 'Walking Dinner' om een idee te krijgen van wat er verwacht werd.
* Opzet maken van pagina's die we verwachten nodig te hebben.
### Taakverdeling
We zijn begonnen met afzonderlijk van elkaar de front- en backend op te zetten, en daarna zijn we samen verder gegaan met de inhoud van elke pagina. We hebben, na de originele opzet, voornamelijk samen gewerkt door middel van voice chat, scherm delen en live share.

# Gebruikte technieken
We hebben ons project opgezet in ASP.NET Core, omdat dit moderner, cross-platform en dus klaar voor de toekomst is. We combineren het met Razor Pages zodat elke pagina een losse unit is, die één, herkenbare taak heeft. Dit vonden wij een fijne structuur geven.

![https://hackernoon.com/hn-images/1*XGFMlY2nwVoFOCUU6jpw-A.png](https://hackernoon.com/hn-images/1*XGFMlY2nwVoFOCUU6jpw-A.png)
# Opbouw en documentatie van code 
![https://mermaid.ink/img/eyJjb2RlIjoiZ3JhcGggVERcbkFbSG9vZmRwYWdpbmFdIC0tPiBCKE5pZXV3IGRpbmVyKVxuQiAtLT4gQyhBZG1pbmlzdHJhdG9yIGtyaWpndCBlZW4gZW1haWwgbWV0IGNvZGUpXG5CIC0tPiBYWy9UaXRlbCwgYmVzY2hyaWp2aW5nLCB2ZXJ6YW1lbHB1bnQsPGJyIC8-cHJpanMsIGdlZ2V2ZW5zIHZhbiBhYW5tYWtlci9dXG5DIC0tPiBEKEJlaGVlcmRlcnMgcGFnaW5hKVxuRCAtLT4gRShEaW5lciBhYW5wYXNzZW4pXG5EIC0tPiBGKEdhc3QgdWl0bm9kaWdlbilcbkYgLS0-IEcoR2FzdCBrcmlqZ3QgZW1haWwgbWV0IGNvZGUpXG5HIC0tPiBIKFwiR2FzdCBrYW4gaW5mb3JtYXRpZSBpbnZ1bGxlbiBvLmEuPGJyIC8-IzQwO25hYW0sIGFkcmVzLCBkaWVldHdlbnNlbiwgcGx1cyAxIzQxO1wiKVxuRCAtLT58TmEgdWl0ZXJzdGUgaW5zY2hyaWpmZGF0dW18IEkoQWRtaW5pc3RyYXRvciBraWVzdCBnYW5nZW48YnIgLz5vcCBiYXNpcyB2YW4gYWFudGFsIGFhbm1lbGRpbmdlbilcbkkgLS0-IEooU2VydmVyIG1hYWt0IHZlcmRlbGluZywgc3R1dXJ0IGdhc3RlbiBlbWFpbDxiciAvPm1ldCB3YW5uZWVyIHppaiBtb2V0ZW4ga29rZW4gZW4gZ2VuZXJlZXJ0PGJyIC8-cGRmIG1ldCBkaW5lcmdlZ2V2ZW5zLikiLCJtZXJtYWlkIjp7InRoZW1lIjoiZGVmYXVsdCJ9LCJ1cGRhdGVFZGl0b3IiOmZhbHNlfQ](https://mermaid.ink/img/eyJjb2RlIjoiZ3JhcGggVERcbkFbSG9vZmRwYWdpbmFdIC0tPiBCKE5pZXV3IGRpbmVyKVxuQiAtLT4gQyhBZG1pbmlzdHJhdG9yIGtyaWpndCBlZW4gZW1haWwgbWV0IGNvZGUpXG5CIC0tPiBYWy9UaXRlbCwgYmVzY2hyaWp2aW5nLCB2ZXJ6YW1lbHB1bnQsPGJyIC8-cHJpanMsIGdlZ2V2ZW5zIHZhbiBhYW5tYWtlci9dXG5DIC0tPiBEKEJlaGVlcmRlcnMgcGFnaW5hKVxuRCAtLT4gRShEaW5lciBhYW5wYXNzZW4pXG5EIC0tPiBGKEdhc3QgdWl0bm9kaWdlbilcbkYgLS0-IEcoR2FzdCBrcmlqZ3QgZW1haWwgbWV0IGNvZGUpXG5HIC0tPiBIKFwiR2FzdCBrYW4gaW5mb3JtYXRpZSBpbnZ1bGxlbiBvLmEuPGJyIC8-IzQwO25hYW0sIGFkcmVzLCBkaWVldHdlbnNlbiwgcGx1cyAxIzQxO1wiKVxuRCAtLT58TmEgdWl0ZXJzdGUgaW5zY2hyaWpmZGF0dW18IEkoQWRtaW5pc3RyYXRvciBraWVzdCBnYW5nZW48YnIgLz5vcCBiYXNpcyB2YW4gYWFudGFsIGFhbm1lbGRpbmdlbilcbkkgLS0-IEooU2VydmVyIG1hYWt0IHZlcmRlbGluZywgc3R1dXJ0IGdhc3RlbiBlbWFpbDxiciAvPm1ldCB3YW5uZWVyIHppaiBtb2V0ZW4ga29rZW4gZW4gZ2VuZXJlZXJ0PGJyIC8-cGRmIG1ldCBkaW5lcmdlZ2V2ZW5zLikiLCJtZXJtYWlkIjp7InRoZW1lIjoiZGVmYXVsdCJ9LCJ1cGRhdGVFZGl0b3IiOmZhbHNlfQ)

### Opbouw van de database
![https://raw.githubusercontent.com/Hansie211/WalkingDinner/master/assets/diagram.png](https://raw.githubusercontent.com/Hansie211/WalkingDinner/master/assets/diagram.png)

### Opbouw van het schema
#### Ronde 1
| Groep A| Groep B| Groep C | Groep D | Groep E
|--|--|--|--|--|
| **01** | 02 | 03 | 04 | 05
| 06 | **07** | 08 | 09 | 10
| 11 | 12 | **13** | 14 | 15
| 16 | 17 | 18 | **19** | 20
| 21 | 22 | 23 | 24 | **25**

#### Ronde 2
| Groep A| Groep B| Groep C | Groep D | Groep E
|--|--|--|--|--|
| **01** | 02 | 03 | 04 | 05
| 07 | **08** | 09 | 10 | 06 
| 13 | 14 | **15** | 11 | 12 
| 19 | 20 | 16 | **17** | 18 
| 25 | 21 | 21 | 23 | **24** 

#### Ronde 3
| Groep A| Groep B| Groep C | Groep D | Groep E
|--|--|--|--|--|
| 01 | 02 | 03 | 04 | 05
| 08 | 09 | 10 | 11 | 07 
| 15 | 11 | 12 | 13 | 14 
| 17 | 18 | 19 | 20 | 16 
| 24 | 25 | 21 | 22 | 23 


# Werkt het eind-product
De site kan volledig worden doorlopen, een diner kan functioneel worden gebruikt. Wat wij graag nog hadden toegevoegd is:
* Betalen via Mollie is opgenomen in de code maar niet af gemaakt en kan dus alleen in test modus.
* De site stuurt geen herinneringen als de uiterlijke aanmeld datum is verlopen en de rolverdeling kan worden gemaakt.
* Het geld zou verdeeld kunnen worden in percentages per gerecht, zodat een hoofdgerecht bijvoorbeeld meer krijgt dan een kleine amuse.
* Een tijdsverdeling per gerecht; meer tijd voor een groot gerecht, minder voor een klein gerecht.

# Manier van presenteren
#### Aan de hand van de read.me op Github.
