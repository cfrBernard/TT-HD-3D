# AI & Simulation v.16 – Overview (Changelog v15 -> v16)

Cette feature couvre :

- la simulation du plateau pour l’IA (`BoardSimulation`)
- le calcul des meilleurs coups (`AIController`)
- la séparation logique vs UI
- la compatibilité avec les règles existantes (`RuleEngine` + règles)

---

## 1️⃣ Objectif principal

**Problème initial** : l’IA devait choisir une carte et un slot optimaux, mais pour tester un coup, il ne fallait pas modifier le vrai plateau ni déclencher les animations/UI.

**Solution** :

- Créer une simulation complète du board pour que l’IA puisse tester chaque coup.
- Séparer le code logique de la carte / du board de la partie visuelle (`CardView`, `CardAnimator`).
- Faire en sorte que le même moteur de règles (`RuleEngine`) fonctionne sur le vrai plateau et sur le plateau simulé.

---

## 2️⃣ Composants de simulation

### BoardState

- Représente l’état logique d’un plateau (3x3).
- Stocke uniquement les cartes (`Card[,] slots`) et leur propriétaire.
- Méthodes utiles :

    - `IsEmpty(x, y)`
    - `PlaceCard(x, y, card)`
    - `GetAllCards()`
    - `Clone()` -> permet de dupliquer l’état pour tester des coups sans toucher au vrai plateau.

> 💡 Cette classe est purement logique, pas de MonoBehaviour, pas d’UI.

#

### BoardConverter

- Méthode `ToState(BoardManager board)` -> transforme le plateau réel en `BoardState` pour la simulation.
- Permet de faire tourner l’IA sur un snapshot du plateau actuel.

### SimBoardManager : IBoard

- Adaptateur pour faire fonctionner le moteur de règles sur un `BoardState`.
- Implémente `IBoard` (interface générique du plateau).
- Méthodes principales :

    - `GetSlot(x, y)` -> renvoie un SimSlot
    - `TryPlaceCard(x, y, card)` -> place une carte logique
    - `GetFreeSlots()`

> 💡 Permet de réutiliser toutes les règles existantes pour la simulation sans toucher au vrai plateau.

#

### SimSlot : IBoardSlot

- Wrapper autour d’une carte dans `BoardState`.
- Fournit `X, Y, Occupant, IsEmpty`.
- Sert de “slot logique” pour les règles et l’IA.

---

## Nouvelles interfaces

### IBoard

Interface abstraite pour un plateau :

```
IBoardSlot GetSlot(int x, int y);
IEnumerable<IBoardSlot> GetAllSlots();
bool TryPlaceCard(int x, int y, Card card);
IBoardSlot GetSlotOfCard(Card card);
IEnumerable<(int x, int y)> GetFreeSlots();
```

> 💡 Permet de traiter BoardManager réel et SimBoardManager de manière identique pour le moteur de règles.

#

### IBoardSlot

Interface abstraite pour un slot :

```
int X { get; }
int Y { get; }
Card Occupant { get; }
bool IsEmpty { get; }
```

---

## 4️⃣ Modifications autour des règles

### RuleEngine / Règles

- Anciennement, toutes les règles (`BasicCaptureRule`, `SameRule`, etc.) prenaient `BoardManager`.
- **Problème** : ne fonctionnait pas avec le plateau simulé.
- **Solution** : toutes les règles prennent désormais `IBoard` et `IBoardSlot`.
- Les règles ne manipulent que la logique (`Owner`) et jamais la vue (`CardView`).
- L’UI est mise à jour uniquement via `Card.SetOwner()` pour les vraies cartes sur le vrai plateau.

### CardViewRegistry

Avant : `GetView` faisait un `Debug.LogError` si aucune vue trouvée.
**Problème** : en simulation, les clones n’ont pas de vue → spam d’erreurs.
**Correction** : `GetView` devient safe, silent ou warning selon contexte (`Application.isPlaying`).

---

## 5️⃣ AIController & simulation

### Flow IA

1. Convertit le plateau réel en `BoardState` : `var baseState = BoardConverter.ToState(board)`
2. Pour chaque carte en main et chaque slot libre :
    - Clone le `BoardState` → `simState`
    - Crée un `SimBoardManager(simState)`
    - Place une carte clone `simCard` sur le simBoard
    - Applique le `RuleEngine.Resolve(simManager, x, y, simCard)`
    - Évalue le score (`EvaluateBoard(simState, player)`)
3. Garde le meilleur coup et le joue sur le vrai plateau (`BoardManager.TryPlaceCard`).

### Avantages

- L’IA peut simuler tous les coups possibles sans toucher à l’UI réelle.
- Réutilisation totale des règles existantes.
- Séparation claire logique vs visuel.

---

## 6️⃣ BoardManager réel

- Implémente aussi `IBoard` via implémentation explicite → peut être passé aux règles pour un traitement uniforme.
- Déclenche l’UI via `Card.SetOwner` → animations / flips.
- Enregistre les `CardView` dans `CardViewRegistry`.

---

## 7️⃣ Points d’amélioration / axes possibles

**1. Refacto BoardManager** : si dès le début il avait été conçu pour supporter IBoard / IBoardSlot, la simulation aurait été plus simple et moins de wrappers nécessaires.

**2. IA plus intelligente** :
    - Évaluer combos / flips multiples
    - Prendre en compte la probabilité de l’adversaire
    - Implémenter un mini-MCTS ou minimax simplifié

**3. Séparation UI/Logic encore plus stricte** :
    - Les règles ne devraient jamais toucher au MonoBehaviour ou à CardView.

**4. Optimisation simulation** :
    - Réutiliser un pool de clones pour éviter allocations à chaque coup
    - Limiter les deep clones si pas nécessaire

**5. Debug / logging IA** :
    - Ajouter un “IA debug mode” pour tracer toutes les simulations sans polluer les logs réels.

--- 

## 8️⃣ ASCII Diagramme

```
AIController
   |
   | -- BoardConverter --> BoardState (snapshot)
   | -- SimBoardManager <-- BoardState
   |        |
   |        |-- SimSlot
   |        |-- RuleEngine.Resolve(IBoard)
   |
   + --> EvaluateBoard(simState)
   |
   + --> BoardManager (vrai) --> place carte + UI updates
```

> - `SimBoardManager` = clone pour tester
> - `BoardManager` = vrai plateau
> - Règles et AI ne savent pas si elles travaillent sur le vrai plateau ou la simulation → grâce à `IBoard`.

---