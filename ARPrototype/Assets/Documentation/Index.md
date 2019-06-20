# La Fabulerie - AR Prototype - mai 2019

# Présentation

Ce projet a été initialement réalisé dans le cadre d'un stage de 4 semaines au sein de la Fabulerie entre avril et mai 2019. La demande globale porta sur une application de réalité augmenté (AR) pouvant remplacer des images du réel par d'autres images virtuelles statique ou en mouvement. La partie lié aux vissages augmentés est venu un peu plus tard.

Malgrè des essais infructeux pour créer un framework opensource à l'aide d'OpenCV 4, et des problèmes liés à l'utilisation du framework open source ARToolKit, j'ai décidé de choisir le framework ARCode de Google.

Google ARCode est gratuit d'utiliation en fonctionnant aussi bien sur Android que iOS tout en étant intégrable facilement dans des projets Unity. L'avantage selon du framewok de Google est qu'il est maintenu par un des gens compétents qui ne risque pas d'être abandonner rapidement car il constitu un réposne au ARKit de Apple et qu'il sert de framework AR de base sur la plateforme Android.

Le projet remis à la Fabuerie à la mi-mai 2019 est fonctionel mais aurait bessoin de semaines de dévellopement en plus pour y ajouter encore des fonctions essentielles, refactorisé (car de nombreux bloc se répète) et commenté entièrement le code, ainsi que rendre cette documentation encore plus complète.

Ce projet utilise Unity en version 2018.3.14f1 et ARCode en version 1.8.0, car la version version 1.8.0 de Google ARCode est incompatible avec des versions supérieurs d'Unity ([> github](https://github.com/google-ar/arcore-unity-sdk/issues/550#issuecomment-492383314)), bien qu'au moment où cette documentation est écrite, la version 1.9.0 vient de sortir en apportant quelques nouveautés, à essayer.

# Ressource

[Comparatif des principaux frameworks de réalité augmenté en 2018-2019](https://thinkmobiles.com/blog/best-ar-sdk-review/)

[Google ARCode - présentation principale](https://developers.google.com/ar/)

[Google ARcode - présentation dévellopeur](https://developers.google.com/ar/develop/)

[Google ARcode - API reference ](https://developers.google.com/ar/reference/)

# Mode d'emploi

## Augmented Image

Remplace une image détecteur par une image cible statique ou en mouvement avec possiblité de jouer sur le décallage de l'image cible par rapport au detecteur en appuyant sur l'image et en la faisant glisser avec un doigt.

1. Dans Unity, il faut sélectionner les images et faire un clique droit pour crée sous le menu "GoogleARCode" une "AugmentedImageDatabase". Pour de bonne performance voir [la documentation explicative de Google](https://developers.google.com/ar/develop/unity/augmented-images/).

2. Puis, créer une "MatchingImageDatabase" dans laquelle il faudra renseigner "AugmentedImageDatabase" utilisé. Une fois cela fait, il ne reste plus qu'à associer pour chaque detecteur une image.

3. Il ne reste plus qu'à associé "AugmentedImageDatabase" au "ARCoreSessionConfig" qui doit être référencé dans "ARCoreSession".

4. Ajouter un GameObject et lui joindre le composant "TrackedImageManager" puis référencé "MatchingImageDatabase".

## Augmented Face

Colle sur le vissage un masque créer au préalable dans "Augmented Face Creator". La version 1.8.0 limite à 1 vissage maximum. Les vissages créée apparaisent en bas, il suffit d'en faire glisser un vers le haut pour l'utiliser et de le glisser vers le bas pour le supprimer à l'aide d'un doigt.

Il reste à ajouter l'utilisation de vidéos, de gifs, et la possibilité d'ajouter des objets en plus sur le vissage.

## Augmented Face Creator

Aide à la création de vissage en utilisant massivement l'utiliation des doigts.

# API

A faire.

# A TRIER

[QuickStart IOS](https://developers.google.com/ar/develop/unity/quickstart-ios)
