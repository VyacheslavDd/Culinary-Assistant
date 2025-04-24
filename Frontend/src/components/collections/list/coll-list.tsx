import { CardCollMax } from '../card-max/card-coll-max';
import { Filter } from '../filter/filter';
import styles from './coll-list.module.scss';

type Props = {
    title: string;
};

export function CollList(props: Props) {
    const { title } = props;
    return (
        <div className={styles.mainContainer}>
            <h2 className={styles.title}>{title}</h2>
            <Filter />
            <ul className={styles.list}>
                <li>
                    <CardCollMax />
                </li>
                <li>
                    <CardCollMax />
                </li>
                <li>
                    <CardCollMax />
                </li>
                <li>
                    <CardCollMax />
                </li>
                <li>
                    <CardCollMax />
                </li>
            </ul>
        </div>
    );
}
