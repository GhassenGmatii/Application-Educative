package tp.chaima.demo.model;

import jakarta.persistence.*;
import lombok.*;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
@Entity
@Table(name = "user")
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    private String name;
    private String email;

    // Plusieurs users appartiennent à une classe
    @ManyToOne
    @JoinColumn(name = "classe_id")   // clé étrangère dans la table user
    private Classe classe;
}