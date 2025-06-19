import { ShortCollection } from 'types/short-collection.type';
import styles from './add-to-collection.module.scss';

type AddToCollectionOverlayProps = {
    collections: ShortCollection[];
    onSelectCollection: (collectionId: string) => void;
};

export function AddToCollectionOverlay({
    collections,
    onSelectCollection,
}: AddToCollectionOverlayProps) {
    return (
        <ul className={styles.mainContainer}>
            {collections.map((collection) => (
                <li
                    key={collection.id}
                    className={styles.li}
                    onClick={() => onSelectCollection(collection.id)}
                >
                    {collection.title}
                </li>
            ))}
        </ul>
    );
}